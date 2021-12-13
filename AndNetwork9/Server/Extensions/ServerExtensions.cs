using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Extensions;

namespace AndNetwork9.Server.Extensions;

public static class ServerExtensions
{
    public static IEnumerable GetShort(this IEnumerable<Member> members)
    {
        return members.Select(x => new
        {
            x.Id, x.Nickname, x.RealName, x.Rank, x.Direction, x.SquadNumber, x.SquadPartNumber,
        });
    }

    public static IQueryable GetShort(this IQueryable<Member> members)
    {
        return members.Select(x => new
        {
            x.Id, x.Nickname, x.RealName, x.Rank, x.Direction, x.SquadNumber, x.SquadPartNumber,
        });
    }


    public static IEnumerable GetPublicShort(this IEnumerable<Member> members)
    {
        return members.Select(x => new
        {
            x.DiscordId,
            x.Nickname,
            x.RealName,
            x.Rank,
            RankIcon = x.Rank.GetRankIcon(),
            RankName = x.Rank.GetRankName(),
            FullNickname = x.ToString(),
            TagNickname = x.ToTagString(),
            x.Direction,
            DirectionName = x.Direction.GetName(),
            x.SquadNumber,
            SquadName = x.SquadPart?.Squad.ToString(),
            x.SquadPartNumber,
            x.CommanderLevel,
            CommanderLevelIcon = x.CommanderLevel.GetSquadCommanderIcon(),
            CommanderLevelName = x.CommanderLevel.GetSquadCommanderName(),
            x.LastChanged,
        });
    }
}