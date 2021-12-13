using System;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace AndNetwork9.Server.Auth.Attributes;

public class MinRankAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
{
    public const string POLICY_PREFIX = "MinRank-";

    public MinRankAuthorizeAttribute(Rank rank = Rank.Neophyte) => Rank = rank;

    public Rank Rank { get; }

    public string PolicyName => $"{POLICY_PREFIX}{Rank:G}";

    public bool Pass(Member member) => member.Rank >= Rank;

    public static bool TryParse(string value, out IAuthorizationRequirement policyName)
    {
        if (Enum.TryParse(value, out Rank rank))
        {
            policyName = new MinRankAuthorizeAttribute(rank);
            return true;
        }

        policyName = null!;
        return false;
    }
}