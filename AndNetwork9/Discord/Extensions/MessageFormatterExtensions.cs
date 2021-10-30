using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Extensions;

namespace AndNetwork9.Discord.Extensions;

public static class MessageFormatterExtensions
{
    private static readonly IImmutableDictionary<string, Func<Member, string>> MemberFormatters =
        ImmutableDictionary<string, Func<Member, string>>.Empty.AddRange(
            new Dictionary<string, Func<Member, string>>
            {
                {
                    "{full_nickname}", member => member.ToString()
                },
                {
                    "{nickname}", member => member.Nickname
                },
                {
                    "{real_name}", member => member.RealName ?? string.Empty
                },
                {
                    "{rank_name}", member => member.Rank.GetRankName()
                },
                {
                    "{rank_icon}", member => member.Rank.GetRankIcon() ?? string.Empty
                },
                {
                    "{discord_mention}", member => $"<@{member.DiscordId}>"
                },
                {
                    "{direction}", member => member.Direction.GetName()
                },
                {
                    "{squad_full_name}", member => member.SquadPart?.Squad?.ToString() ?? string.Empty
                },
                {
                    "{squad_name}", member => member.SquadPart?.Squad?.Name ?? string.Empty
                },
                {
                    "{squad_number}", member => member.SquadPart?.Number.ToRoman() ?? string.Empty
                },
            });

    public static string FormatString(this string value, Member member)
    {
        return MemberFormatters.Aggregate(value,
            (current, formatter) => current.Replace(formatter.Key, formatter.Value(member)));
    }
}