using System;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Server.Auth;

public class ClanPolicyProvider : IAuthorizationPolicyProvider, IAuthorizationHandler
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ClanPolicyProvider(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ClanDataContext? data = (ClanDataContext?)scope.ServiceProvider.GetService(typeof(ClanDataContext));
        if (data is null) throw new ApplicationException();

        Member? member = await context.User.GetCurrentMember(data).ConfigureAwait(false);
        AuthSession? session = await context.User.GetCurrentSession(data).ConfigureAwait(false);
        if (member is null
            || session is null
            || session.ExpireTime < DateTime.UtcNow
            || session.Member.Id != member.Id)
        {
            context.Fail();
            return;
        }

        foreach (IAuthorizationRequirement requirement in context.PendingRequirements)
            if (requirement is IAuthPass authPass)
                if (authPass.Pass(member)) context.Succeed(requirement);
                else context.Fail();
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicyBuilder builder =
            new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim(AuthExtensions.MEMBER_ID_CLAIM_NAME)
                .RequireClaim(AuthExtensions.SESSION_ID_CLAIM_NAME);

        if (!policyName.StartsWith(MinRankAuthorizeAttribute.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase)
            && MinRankAuthorizeAttribute.TryParse(policyName[MinRankAuthorizeAttribute.POLICY_PREFIX.Length..],
                out IAuthorizationRequirement policy)) builder.AddRequirements(policy);
        else if (!policyName.StartsWith(DirectionAuthorizeAttribute.POLICY_PREFIX,
                     StringComparison.OrdinalIgnoreCase)
                 && DirectionAuthorizeAttribute.TryParse(
                     policyName[DirectionAuthorizeAttribute.POLICY_PREFIX.Length..],
                     out policy)) builder.AddRequirements(policy);

        return Task.FromResult(builder.Build())!;
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync() => await
        Task.FromResult(
            new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build()).ConfigureAwait(false);

    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        await Task.FromResult(default(AuthorizationPolicy)).ConfigureAwait(false);
}