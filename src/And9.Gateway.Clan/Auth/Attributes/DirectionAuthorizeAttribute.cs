using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace And9.Gateway.Clan.Auth.Attributes;

public class DirectionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
{
    public const string POLICY_PREFIX = "Direction-";

    public DirectionAuthorizeAttribute(params Direction[] directions) => Directions = directions;

    public Direction[] Directions { get; }

    public string PolicyName
    {
        get { return $"{POLICY_PREFIX}{string.Join(',', Directions.OrderBy(x => x).Select(x => x.ToString("G")))}"; }
    }

    public bool Pass(IMember member)
    {
        return Directions.Any(x => x == member.Direction);
    }

    public static bool TryParse(string value, out IAuthorizationRequirement policyName)
    {
        List<Direction> directions = new();
        foreach (string directionName in value.Split(','))
            if (Enum.TryParse(directionName, out Direction direction))
            {
                directions.Add(direction);
            }
            else
            {
                policyName = null!;
                return false;
            }

        policyName = new DirectionAuthorizeAttribute(directions.ToArray());
        return true;
    }
}