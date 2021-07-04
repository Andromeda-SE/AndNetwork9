using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AndNetwork9.Shared;

namespace AndNetwork9.Server.Extensions
{
    public static class ServerExtensions
    {
        public static IEnumerable GetShort(this IEnumerable<Member> members)
        {
            return members.Select(x => new { x.Id, x.Nickname, x.RealName, x.Rank, x.Direction, x.SquadNumber });
        }

        public static IQueryable GetShort(this IQueryable<Member> members)
        {
            return members.Select(x => new { x.Id, x.Nickname, x.RealName, x.Rank, x.Direction, x.SquadNumber });
        }
    }
}