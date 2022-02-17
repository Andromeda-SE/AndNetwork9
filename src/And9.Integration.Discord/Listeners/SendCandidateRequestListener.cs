using System.Runtime.CompilerServices;
using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Database.Models;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Interfaces;
using Discord;
using Discord.WebSocket;
using IConnection = RabbitMQ.Client.IConnection;

namespace And9.Integration.Discord.Listeners;

public class SendCandidateRequestListener : BaseRabbitListenerWithoutResponse<ICandidateRequest>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SendCandidateRequestListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithoutResponse<ICandidateRequest>> logger,
        IServiceScopeFactory serviceScopeFactory)
        : base(connection, SendCandidateRequestSender.QUEUE_NAME, logger) => _serviceScopeFactory = serviceScopeFactory;

    public override async Task Run(ICandidateRequest request)
    {
        if (request.AuxiliarySquad is not null)
        {
            Logger.LogWarning("Request with AuxiliarySquad not supported");
            return;
        }

        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);

        DiscordDataContext discordDataContext = scope.ServiceProvider.GetRequiredService<DiscordDataContext>();
        Channel? channel = discordDataContext.Channels.FirstOrDefault(x => x.Type == DiscordChannelType.Text && x.DiscordChannelFlags.HasFlag(DiscordChannelFlags.CandidatesChannel));
        if (channel is null)
        {
            Logger.LogError("Channel with flag «CandidatesChannel» not found");
            return;
        }

        DiscordBot bot = scope.ServiceProvider.GetRequiredService<DiscordBot>();
        SocketGuild? guild = bot.GetGuild(bot.GuildId);
        SocketTextChannel? discordChannel = guild?.GetTextChannel(channel.DiscordId);
        if (guild is null || discordChannel is null)
        {
            Logger.LogError("Channel with id «{0}» not found in Discord", channel.DiscordId);
            return;
        }

        Embed embed = new EmbedBuilder()
            .WithTitle("Новая заявка в клан")
            .WithFields(new[]
            {
                new EmbedFieldBuilder()
                    .WithName("Никнейм")
                    .WithValue(request.Nickname),
                new EmbedFieldBuilder()
                    .WithName("Имя")
                    .WithValue(request.Nickname),
                new EmbedFieldBuilder()
                    .WithName("Steam")
                    .WithValue(request.GetSteamLink()),
                new EmbedFieldBuilder()
                    .WithName("ВКонтакте")
                    .WithValue(request.GetVkLink() ?? "—"),
                new EmbedFieldBuilder()
                    .WithName("Возраст")
                    .WithValue(request.Age.HasValue ? request.Age.Value : "—"),
                new EmbedFieldBuilder()
                    .WithName("Часов в игре")
                    .WithValue(request.HoursCount.HasValue ? request.HoursCount.Value : "—"),
                new EmbedFieldBuilder()
                    .WithName("Часовой пояс")
                    .WithValue(request.TimeZone is not null
                        ? $"{request.TimeZone.Id} (UTC{(request.TimeZone.BaseUtcOffset >= TimeSpan.Zero ? '+' : '-')}{request.TimeZone.GetUtcOffset(DateTime.UtcNow).Duration():hh\\:mm})"
                        : "—"),
                new EmbedFieldBuilder()
                    .WithName("Рекомендация")
                    .WithValue(request.Recommendation ?? "—"),
            })
            .WithDescription(request.Description)
            .WithCurrentTimestamp()
            .WithAuthor(guild.GetUser(request.DiscordId))
            .Build();

        await discordChannel.SendMessageAsync("@everyone" + Environment.NewLine + request.GetDiscordMention(), embed: embed).ConfigureAwait(false);
    }
}