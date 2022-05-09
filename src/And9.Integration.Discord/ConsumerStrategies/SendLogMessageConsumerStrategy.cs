using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Models;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using Discord.WebSocket;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(SendLogMessageSender.QUEUE_NAME)]
public class SendLogMessageConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<string>
{
    private readonly DiscordBot _bot;
    private readonly DiscordDataContext _discordDataContext;
    private readonly ILogger<SendLogMessageConsumerStrategy> _logger;

    public SendLogMessageConsumerStrategy(DiscordBot bot, DiscordDataContext discordDataContext, ILogger<SendLogMessageConsumerStrategy> logger)
    {
        _bot = bot;
        _discordDataContext = discordDataContext;
        _logger = logger;
    }

    public async ValueTask ExecuteAsync(string arg)
    {
        Channel? channel = _discordDataContext.Channels.FirstOrDefault(x => x.DiscordChannelFlags.HasFlag(DiscordChannelFlags.AutoAnnouncement));
        if (channel is null)
        {
            _logger.LogError("AutoAnnouncement channel not found");
            return;
        }

        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        SocketTextChannel? discordChannel = guild.GetTextChannel(channel.DiscordId);
        await discordChannel.SendMessageAsync(arg).ConfigureAwait(false);
    }
}