using And9.Service.Core.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace And9.Gateway.Clan.Auth.Attributes;

public class NotInSquadAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
{
    public bool Pass(IMember member) => member.SquadNumber is null;
}