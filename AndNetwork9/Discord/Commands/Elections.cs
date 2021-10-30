using System.Net;
using System.Threading.Tasks;
using AndNetwork9.Discord.Permissions;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Elections;
using AndNetwork9.Shared.Enums;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Discord.Commands;

[Group(nameof(Elections))]
public class Elections : Base
{
    private readonly NextStageSender _nextStageSender;
    private readonly RegisterSender _registerSender;

    public Elections(DiscordBot bot, ClanDataContext data, NextStageSender nextStageSender,
        RegisterSender registerSender) : base(bot, data)
    {
        _nextStageSender = nextStageSender;
        _registerSender = registerSender;
    }

    [Command(nameof(NextStage))]
    [Summary("Перехоит к следующей стадии выборов")]
    [MinRankPermission(Rank.FirstAdvisor)]
    public async Task NextStage()
    {
        await _nextStageSender.CallAsync(ElectionStage.None).ConfigureAwait(false);
        await ReplyAsync("Стадия пройдена").ConfigureAwait(false);
    }

    [Command(nameof(Register))]
    [Alias("reg")]
    [Summary("Региструет на выборах как кандидата участника клана")]
    [MinRankPermission(Rank.FirstAdvisor)]
    public async Task Register(int id)
    {
        try
        {
            await _registerSender.CallAsync(id).ConfigureAwait(false);
        }
        catch (FailedCallException e)
        {
            await ReplyAsync(e.Code switch
            {
                HttpStatusCode.NotFound => "Игрок не найден",
                HttpStatusCode.Forbidden => "Игрок не соответствует требованям",
                HttpStatusCode.Gone => "На данный момент регистрация на выборы невозможна",
                HttpStatusCode.AlreadyReported => "Игрок уже зарегистрирован",
                _ => "Ошибка при выполнении команды",
            }).ConfigureAwait(false);
        }

        await ReplyAsync("Игрок зарегистрирован").ConfigureAwait(false);
    }

    [Command(nameof(Register))]
    [Alias("reg")]
    [Summary("Региструет на выборах как кандидата участника клана")]
    [MinRankPermission(Rank.Assistant)]
    public async Task Register()
    {
        Shared.Member member = await Data.Members.FirstAsync(x => x.DiscordId == Context.Message.Author.Id)
            .ConfigureAwait(false);
        await Register(member.Id).ConfigureAwait(false);
    }
}