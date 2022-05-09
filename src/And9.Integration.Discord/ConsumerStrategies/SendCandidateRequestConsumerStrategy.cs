using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Models;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Interfaces;
using Discord;
using Discord.WebSocket;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(SendCandidateRequestSender.QUEUE_NAME)]
public class SendCandidateRequestConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<ICandidateRequest>
{
    private readonly DiscordBot _bot;
    private readonly DiscordDataContext _discordDataContext;
    private readonly ILogger<SendCandidateRequestConsumerStrategy> _logger;

    public SendCandidateRequestConsumerStrategy(DiscordDataContext discordDataContext, DiscordBot bot, ILogger<SendCandidateRequestConsumerStrategy> logger)
    {
        _discordDataContext = discordDataContext;
        _bot = bot;
        _logger = logger;
    }

    public async ValueTask ExecuteAsync(ICandidateRequest request)
    {
        if (request.AuxiliarySquad is not null)
        {
            _logger.LogWarning("Request with AuxiliarySquad not supported");
            return;
        }

        Channel? channel = _discordDataContext.Channels.FirstOrDefault(x => x.Type == DiscordChannelType.Text && x.DiscordChannelFlags.HasFlag(DiscordChannelFlags.CandidatesChannel));
        if (channel is null)
        {
            _logger.LogError("Channel with flag «CandidatesChannel» not found");
            return;
        }

        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        SocketTextChannel? discordChannel = guild?.GetTextChannel(channel.DiscordId);
        if (guild is null || discordChannel is null)
        {
            _logger.LogError("Channel with id «{0}» not found in Discord", channel.DiscordId);
            return;
        }

        TimeSpan? timeZoneOffset = request.TimeZone?.GetUtcOffset(DateTime.UtcNow);
        Embed embed = new EmbedBuilder()
            .WithTitle("Новая заявка в клан")
            .WithFields(new EmbedFieldBuilder()
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
                    .WithValue(timeZoneOffset is not null
                        ? $"{request.TimeZone!.Id} (UTC{(timeZoneOffset.Value >= TimeSpan.Zero ? '+' : '-')}{timeZoneOffset.Value.Duration():hh\\:mm})"
                        : "—"),
                new EmbedFieldBuilder()
                    .WithName("Рекомендация")
                    .WithValue(request.Recommendation ?? "—"))
            .WithDescription(request.Description)
            .WithCurrentTimestamp()
            .WithAuthor(guild.GetUser(request.DiscordId))
            .Build();

        await discordChannel.SendMessageAsync("@everyone" + Environment.NewLine + request.GetDiscordMention(), embed: embed).ConfigureAwait(false);
    }
}