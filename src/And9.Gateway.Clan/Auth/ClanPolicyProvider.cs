using System.Security.Claims;
using And9.Lib.API;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using StackExchange.Redis;

namespace And9.Gateway.Clan.Auth;

public class ClanPolicyProvider : IAuthorizationHandler
{
    private readonly ReadMemberByIdSender _readMemberByIdSender;
    private readonly IConnectionMultiplexer _redis;

    public ClanPolicyProvider(IConnectionMultiplexer redis, ReadMemberByIdSender readMemberByIdSender)
    {
        _redis = redis;
        _readMemberByIdSender = readMemberByIdSender;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        string? userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out int memberId))
        {
            context.Fail();
            return;
        }

        ValueTask<Member?> gettingMember = _readMemberByIdSender.CallAsync(memberId);

        IDatabase? db = _redis.GetDatabase(AuthOptions.REDIS_DATABASE_ID, new());
        string? tokenId = context.User.FindFirst(ClaimTypes.Hash)?.Value;
        try
        {
            if (string.IsNullOrEmpty(tokenId) || !await db.SetContainsAsync(userId, tokenId).ConfigureAwait(false))
            {
                context.Fail();
                return;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        IMember? member = await gettingMember.ConfigureAwait(false);
        if (member is null)
        {
            context.Fail();
            return;
        }

        foreach (DenyAnonymousAuthorizationRequirement requirement
                 in context.PendingRequirements.OfType<DenyAnonymousAuthorizationRequirement>())
            context.Succeed(requirement);

        foreach (IAuthorizationRequirement requirement in context.PendingRequirements)
            if (requirement is IAuthPass authPass)
                if (authPass.Pass(member)) context.Succeed(requirement);
                else context.Fail();
    }
}