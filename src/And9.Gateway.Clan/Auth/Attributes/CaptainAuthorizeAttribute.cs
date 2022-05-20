using And9.Service.Core.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace And9.Gateway.Clan.Auth.Attributes;

public class CaptainAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
{
    public bool Pass(IMember member) => member.SquadNumber is not null && member.IsSquadCommander && member.SquadPartNumber == 0;
}