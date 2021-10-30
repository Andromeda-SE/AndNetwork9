using System;
using System.Collections.Generic;
using System.Linq;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace AndNetwork9.Server.Auth.Attributes;

public class DirectionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
{
    public const string POLICY_PREFIX = "Direction-";

    public DirectionAuthorizeAttribute(params Direction[] directions)
    {
        Directions = directions.OrderBy(x => x).ToArray();
    }

    public Direction[] Directions { get; }

    public string PolicyName
    {
        get { return $"{POLICY_PREFIX}{string.Join(',', Directions.OrderBy(x => x).Select(x => x.ToString("G")))}"; }
    }

    public bool Pass(Member member)
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