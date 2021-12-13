using System;
using System.Threading.Tasks;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Discord.Channels;
using AndNetwork9.Shared.Backend.Discord.Enums;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Utility;
using Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IConnection = RabbitMQ.Client.IConnection;

namespace AndNetwork9.Discord.Listeners;

public class NewCandidate : BaseRabbitListenerWithoutResponse<CandidateRequest>
{
    private readonly DiscordBot _bot;
    private readonly IConfiguration _configuration;
    private readonly ClanDataContext _data;

    public NewCandidate(IConnection connection, ILogger<BaseRabbitListenerWithoutResponse<CandidateRequest>> logger,
        ClanDataContext data, DiscordBot bot, IConfiguration configuration)
        : base(connection, NewCandidateSender.QUEUE_NAME, logger)
    {
        _data = data;
        _bot = bot;
        _configuration = configuration;
    }

    public override async Task Run(CandidateRequest request)
    {
        Channel? channel =
            await _data.DiscordChannels.FirstOrDefaultAsync(x =>
                x.ChannelFlags.HasFlag(ChannelFlags.CandidatesChannel)).ConfigureAwait(false);
        IUser? discordUser = await _bot.GetUserAsync(request.DiscordId).ConfigureAwait(false);
        IDMChannel? discordChannel = await discordUser.CreateDMChannelAsync().ConfigureAwait(false);
        using IDisposable? typingState = discordChannel.EnterTypingState();
        if (channel is not null)
        {
            Embed? embed = new EmbedBuilder().WithAuthor(discordUser).WithFields(
                new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = "Ник",
                    Value = request.Nickname,
                },
                new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = "Имя",
                    Value = request.RealName,
                },
                new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = "Steam",
                    Value = request.GetSteamLink(),
                },
                new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = "ВК",
                    Value = request.GetVkLink() ?? "—",
                },
                new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = "Возраст",
                    Value = request.Age?.ToString("D") ?? "—",
                },
                new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = "Часов в игре",
                    Value = request.HoursCount?.ToString("D") ?? "—",
                },
                new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = "Часовой пояc",
                    Value = request.TimeZone?.ToString() ?? "—",
                },
                new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = "Источник знаний о клане",
                    Value = request.Recommendation ?? "—",
                }).WithFooter(request.Description).Build();

            await _bot.GetGuild(_bot.GuildId).GetTextChannel(channel.DiscordId)
                .SendMessageAsync("@everyone , новая завяка на вступление в клан!", embed: embed).ConfigureAwait(false);
            await discordChannel.SendMessageAsync(
                "Вы успешно подали заявку в клан «Андромеда»! Рассмотрение заявки занимает от 8 до 48 часов. Если вы считаете, что это ошибка, напишите об этом <@266672873882648577>"
                + Environment.NewLine
                + $"Вам также дан доступ к сайту клана https://{_configuration}/ !").ConfigureAwait(false);
        }
    }
}