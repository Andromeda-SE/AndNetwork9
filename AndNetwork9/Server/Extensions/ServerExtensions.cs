using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AndNetwork9.Shared;

namespace AndNetwork9.Server.Extensions;

public static class ServerExtensions
{
    public static IEnumerable GetShort(this IEnumerable<Member> members)
    {
        return members.Select(x => new
        {
            x.Id, x.Nickname, x.RealName, x.Rank, x.Direction, x.SquadNumber, x.SquadPartNumber, x.SquadCommander,
        });
    }

    public static IQueryable GetShort(this IQueryable<Member> members)
    {
        return members.Select(x => new
        {
            x.Id, x.Nickname, x.RealName, x.Rank, x.Direction, x.SquadNumber, x.SquadPartNumber, x.SquadCommander,
        });
    }

    public static IAsyncEnumerable<dynamic> GetShort(this IAsyncEnumerable<Member> members)
    {
        return members.Select(x => new
        {
            x.Id,
            x.Nickname,
            x.RealName,
            x.Rank,
            x.Direction,
            x.SquadNumber,
            x.SquadPartNumber,
            x.SquadCommander,
        });
    }


    public static IQueryable GetPublicShort(this IQueryable<Member> members)
    {
        return members.Select(x => new
        {
            x.DiscordId,
            x.Nickname,
            x.RealName,
            x.Rank,
            x.Direction,
            x.SquadNumber,
            x.SquadPartNumber,
            x.CommanderLevel
        });
    }
}