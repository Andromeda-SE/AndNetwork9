using AndNetwork9.Shared;
using Microsoft.AspNetCore.Authorization;

namespace AndNetwork9.Server.Auth.Attributes;

public class CommanderAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
{
    public bool Pass(Member member) => member.SquadPart is not null && member.SquadPart.CommanderId == member.Id;
}