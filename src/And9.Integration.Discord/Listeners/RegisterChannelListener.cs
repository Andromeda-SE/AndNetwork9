using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Models;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using Discord.WebSocket;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Listeners;

public class RegisterChannelListener : BaseRabbitListenerWithResponse<Channel, bool>
{
    private readonly DiscordBot _bot;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SyncChannelsSender _syncChannelsSender;

    public RegisterChannelListener(
        IConnection connection,
        ILogger<RegisterChannelListener> logger,
        IServiceScopeFactory serviceScopeFactory,
        DiscordBot bot,
        SyncChannelsSender syncChannelsSender)
        : base(connection, RegisterChannelSender.QUEUE_NAME, logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _bot = bot;
        _syncChannelsSender = syncChannelsSender;
    }

    protected override async Task<bool> GetResponseAsync(Channel channel)
    {
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        if (guild is null) throw new();

        SocketGuildChannel? discordChannel = guild.GetChannel(channel.DiscordId);
        if (discordChannel is null) return false;

        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        DiscordDataContext dataContext = scope.ServiceProvider.GetRequiredService<DiscordDataContext>();
        await dataContext.Channels.AddAsync(channel with
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
        await dataContext.SaveChangesAsync().ConfigureAwait(false);
        await _syncChannelsSender.CallAsync(null!).ConfigureAwait(false);
        return true;
    }
}