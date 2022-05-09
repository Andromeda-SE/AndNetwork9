using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Models;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using Discord.WebSocket;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(RegisterChannelSender.QUEUE_NAME)]
public class RegisterChannelConsumerStrategy : IBrokerConsumerWithResponseStrategy<Channel, bool>
{
    private readonly DiscordBot _bot;
    private readonly DiscordDataContext _dataContext;
    private readonly SyncChannelsSender _syncChannelsSender;

    public RegisterChannelConsumerStrategy(DiscordBot bot, SyncChannelsSender syncChannelsSender, DiscordDataContext dataContext)
    {
        _bot = bot;
        _syncChannelsSender = syncChannelsSender;
        _dataContext = dataContext;
    }

    public async ValueTask<bool> ExecuteAsync(Channel? channel)
    {
        if (channel is null) throw new ArgumentNullException(nameof(channel));
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        if (guild is null) throw new();

        SocketGuildChannel? discordChannel = guild.GetChannel(channel.DiscordId);
        if (discordChannel is null) return false;

        await _dataContext.Channels.AddAsync(channel with
        {
            DiscordId = discordChannel.Id,
            Type = discordChannel switch
            {
                SocketCategoryChannel => throw new NotSupportedException($"Use \"{RegisterChannelCategorySender.QUEUE_NAME}\" queue"),
                SocketNewsChannel => DiscordChannelType.Announcement,
                SocketThreadChannel => DiscordChannelType.Thread,
                SocketTextChannel => DiscordChannelType.Text,
                SocketStageChannel => DiscordChannelType.Stage,
                SocketVoiceChannel => DiscordChannelType.Voice,
                _ => throw new ArgumentOutOfRangeException(nameof(discordChannel)),
            },
            LastChanged = DateTime.UtcNow,
            ConcurrencyToken = Guid.NewGuid(),
        }).ConfigureAwait(false);
        await _dataContext.SaveChangesAsync().ConfigureAwait(false);
        await _syncChannelsSender.CallAsync(null!).ConfigureAwait(false);
        return true;
    }
}