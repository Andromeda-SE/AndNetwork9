using AndNetwork9.Shared;
using Microsoft.AspNetCore.Authorization;

namespace AndNetwork9.Server.Auth.Attributes
{
    public class SquadCommanderAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
    {
        public bool Pass(Member member) => member.Id == member.Squad?.Commander?.Id;
    }
}