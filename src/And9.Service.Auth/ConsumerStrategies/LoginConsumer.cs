using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using And9.Lib.API;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Auth.Abstractions.Models;
using And9.Service.Auth.Database;
using And9.Service.Auth.Database.Models;
using And9.Service.Auth.Senders;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Senders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace And9.Service.Auth.ConsumerStrategies;

[QueueName(LoginSender.QUEUE_NAME)]
public class LoginConsumer : IBrokerConsumerWithResponseStrategy<AuthCredentials, string?>
{
    private readonly AuthDataContext _authDataContext;
    private readonly ReadMemberByNicknameSender _readMemberByNicknameSender;
    private readonly IConnectionMultiplexer _redis;

    public LoginConsumer(IConnectionMultiplexer redis, AuthDataContext authDataContext, ReadMemberByNicknameSender readMemberByNicknameSender)
    {
        _redis = redis;
        _authDataContext = authDataContext;
        _readMemberByNicknameSender = readMemberByNicknameSender;
    }

    public async ValueTask<string?> ExecuteAsync(AuthCredentials request)
    {
        IMember? member = await _readMemberByNicknameSender.CallAsync(request.Nickname).ConfigureAwait(false);
        if (member is null) return null;
        PasswordHash? storedPasswordHash = await _authDataContext.PasswordHashes.FindAsync(member.Id).ConfigureAwait(false);
        if (storedPasswordHash is null || !storedPasswordHash.Hash.SequenceEqual(request.Password.GetPasswordHash())) return null;
        string id = Guid.NewGuid().ToString("N");
        ClaimsIdentity? identity = GetIdentity(member, id);

        DateTime createTime = DateTime.Now;
        JwtSecurityToken jwt = new(
            AuthOptions.ISSUER,
            AuthOptions.AUDIENCE,
            identity.Claims,
            createTime,
            createTime.AddMinutes(AuthOptions.LIFETIME_MINUTES),
            new(AuthOptions.GetSymmetricSecurityIssuerKey(), SecurityAlgorithms.HmacSha256)
        );
        string? encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        IDatabase db = _redis.GetDatabase(AuthOptions.REDIS_DATABASE_ID, new());
        await db.SetAddAsync(member.Id.ToString("D"), id).ConfigureAwait(false);

        return encodedJwt;
    }

    private static ClaimsIdentity GetIdentity(IPublicMember member, in string guid)
    {
        List<Claim> claims = new()
        {
            new(ClaimsIdentity.DefaultNameClaimType, member.Nickname),
            new(ClaimsIdentity.DefaultRoleClaimType, string.Empty),
            new(ClaimTypes.NameIdentifier, member.Id.ToString("D", CultureInfo.InvariantCulture)),
            new(ClaimTypes.Hash, guid),
        };
        ClaimsIdentity claimsIdentity =
            new(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }
}