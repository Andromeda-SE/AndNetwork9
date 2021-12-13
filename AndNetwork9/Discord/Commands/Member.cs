using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndNetwork9.Discord.Permissions;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Discord.Commands;

[Group(nameof(Member))]
public class Member : Base
{
    public Member(DiscordBot bot, ClanDataContext data) : base(bot, data) { }


    [Command(nameof(Set))]
    public async Task Set(string property, [Remainder] string value)
    {
        Shared.Member? member = await Data.Members.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id)
            .ConfigureAwait(false);
        if (member is null)
        {
            await ReplyAsync("Участник не найден").ConfigureAwait(false);
            return;
        }

        switch (property.ToLowerInvariant())
        {
            case "nickname":
                member.Nickname = value;
                break;
            case "realname":
                member.RealName = value;
                break;
            case "timezome":
                TimeZoneInfo timeZone;
                try
                {
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(property);
                }
                catch (TimeZoneNotFoundException)
                {
                    await ReplyAsync("Часовой пояс не найден").ConfigureAwait(false);
                    return;
                }

                member.TimeZone = timeZone;
                break;
            default:
                await ReplyAsync("Поле данных не найдено").ConfigureAwait(false);
                return;
        }

        await Data.SaveChangesAsync().ConfigureAwait(false);
        await ReplyAsync($"Поле {property} изменено на {value}").ConfigureAwait(false);
    }


    [Command(nameof(Info))]
    [Priority(1)]
    public async Task Info()
    {
        Shared.Member? member = await Data.Members.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id)
            .ConfigureAwait(false);
        await Get(member).ConfigureAwait(true);
    }

    [Command(nameof(Info))]
    [MinRankPermission]
    [Priority(2)]
    public async Task Info(int id)
    {
        Shared.Member? member = await Data.Members.FindAsync(id).ConfigureAwait(false);
        await Get(member).ConfigureAwait(true);
    }

    [Command(nameof(Info))]
    [MinRankPermission]
    [Priority(3)]
    public async Task Info(string value)
    {
        Shared.Member? member = Data.Members.FindByString(x => x!.Nickname, value);
        await Get(member).ConfigureAwait(true);
    }

    public async Task Get(Shared.Member? member)
    {
        Shared.Member? caller = Data.Members.FirstOrDefault(x => x.DiscordId == Context.User.Id);
        if (member is null)
        {
            await ReplyAsync("Участник не найден").ConfigureAwait(false);
            return;
        }

        StringBuilder text = new();
        addField("Ник", member.Nickname);
        addField("Имя", member.RealName);
        addField("ID", member.Id.ToString());
        text.AppendLine();
        addField("Ранг", member.Rank.GetRankName());
        switch (member.Direction)
        {
            case > Direction.None:
                addField("Направление", member.Direction.GetName());
                break;
            case < Direction.None:
                addField("Общность игроков", member.Direction.GetName());
                break;
        }

        addField("Отряд", member.SquadPart?.Squad.ToString());
        addField("Командир отряда", member.SquadPart?.Captain.Id == member.Id ? string.Empty : null);
        addField("Отделение", member.SquadPart?.ToString());
        addField("Командир отделения", member.SquadPart?.CommanderId == member.Id ? string.Empty : null);

        text.AppendLine();
        if (member.SteamId is not null)
            addField("Steam", "http://steamcommunity.com/profiles/" + member.SteamId.Value.ToString("D"));
        addField("Discord", $"<@{member.DiscordId:D}>");
        if (member.VkId is not null) addField("ВК", $"https://vk.com/id{member.VkId}");
        if (member.Rank > Rank.None && member.Awards.Any())
        {
            text.AppendLine();
            string awards = string.Join(Environment.NewLine,
                member.Awards.Aggregate(
                    new Dictionary<AwardType, int>(Enum.GetValues<AwardType>().Reverse()
                        .Select(x => new KeyValuePair<AwardType, int>(x, 0))),
                    (counts, award) =>
                    {
                        counts[award.Type]++;
                        return counts;
                    }).Where(x => x.Value > 0).Select(x => $"{x.Key.GetAwardSymbol()} × {x.Value:D}"));
            addField("Награды", member.Awards.Sum(x => x.Points) + Environment.NewLine + awards);
        }

        if (member.TimeZone is not null)
        {
            text.AppendLine();
            addField("Текущее время игрока",
                TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, member.TimeZone).ToString("g", RussianCulture));
            double utcHoursOffset = member.TimeZone.GetUtcOffset(DateTime.UtcNow).TotalHours;
            addField("Смещение от UTC, часов", utcHoursOffset.ToString("G", RussianCulture));
            if (caller?.TimeZone is not null)
                addField("Разница с вами, часов",
                    (utcHoursOffset
                     - member.TimeZone
                         .GetUtcOffset(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, caller.TimeZone))
                         .TotalHours)
                    .ToString("G", RussianCulture));
        }

        await ReplyAsync(text.ToString()).ConfigureAwait(false);

        void addField(string type, string? value)
        {
            if (value is null) return;
            text.Append('*', 2);
            text.Append(type);
            text.Append('*', 2);
            if (value.Length > 0)
            {
                text.Append(':');
                text.Append(' ');
                text.Append(value);
            }

            text.AppendLine();
        }
    }
}