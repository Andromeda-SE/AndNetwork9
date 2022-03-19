using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using And9.Lib.API;
using And9.Lib.Broker;
using And9.Service.Auth.Abstractions.Models;
using And9.Service.Auth.Database;
using And9.Service.Auth.Database.Models;
using And9.Service.Auth.Senders;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Senders;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace And9.Service.Auth.Listeners;

public class LoginListener : BaseRabbitListenerWithResponse<AuthCredentials, string?>
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public LoginListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithResponse<AuthCredentials, string?>> logger,
        IConnectionMultiplexer redis,
        IServiceScopeFactory serviceScopeFactory)
        : base(connection, LoginSender.QUEUE_NAME, logger)
    {
        _redis = redis;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<string?> GetResponseAsync(AuthCredentials request)
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        MemberCrudSender memberCrudSender = scope.ServiceProvider.GetRequiredService<MemberCrudSender>();
        AuthDataContext authDataContext = scope.ServiceProvider.GetRequiredService<AuthDataContext>();
        ;
        IMember? member = await memberCrudSender.ReadByNickname(request.Nickname).ConfigureAwait(false);
        if (member is null) return null;
        PasswordHash? storedPasswordHash = await authDataContext.PasswordHashes.FindAsync(member.Id).ConfigureAwait(false);
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