using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace And9.Gateway.Clan.Auth.Attributes;

public class MinRankAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
{
    public const string POLICY_PREFIX = "MinRank-";

    public MinRankAuthorizeAttribute(Rank rank = Rank.Neophyte) => Rank = rank;

    public Rank Rank { get; }

    public string PolicyName => $"{POLICY_PREFIX}{Rank:G}";

    public bool Pass(IMember member) => member.Rank >= Rank;

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