using System.Threading.Tasks;
using AndNetwork9.Discord.Permissions;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Elections;
using AndNetwork9.Shared.Enums;
using Discord.Commands;

namespace AndNetwork9.Discord.Commands;

[Group(nameof(Elections))]
public class Elections : Base
{
    private readonly NextStageSender _nextStageSender;

    public Elections(DiscordBot bot, ClanDataContext data, NextStageSender nextStageSender) : base(bot, data) =>
        _nextStageSender = nextStageSender;

    [Command(nameof(NextStage))]
    [Summary("Перехоит к следующей стадии выборов")]
    [MinRankPermission(Rank.FirstAdvisor)]
    public async Task NextStage()
    {
        await _nextStageSender.CallAsync(ElectionStage.None).ConfigureAwait(false);
        await ReplyAsync("Стадия пройдена").ConfigureAwait(false);
    }
}