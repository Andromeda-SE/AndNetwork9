using System.Linq;
using AndNetwork9.Shared;
using Microsoft.AspNetCore.Authorization;

namespace AndNetwork9.Server.Auth.Attributes
{
    public class SquadMemberAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthPass
    {
        public bool Pass(Member member)
        {
            return member.Squad is not null && member.Squad.Members.Any(x => x.Id == member.Id);
        }
    }
}