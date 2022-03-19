using System.Runtime.CompilerServices;
using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Models;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using Discord.WebSocket;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Listeners;

public class SendLogMessageListener : BaseRabbitListenerWithoutResponse<string>
{
    private readonly DiscordBot _bot;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SendLogMessageListener(IConnection connection, DiscordBot bot, ILogger<SendLogMessageListener> logger, IServiceScopeFactory serviceScopeFactory)
        : base(connection, SendDirectMessageSender.QUEUE_NAME, logger)
    {
        _bot = bot;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task Run(string arg)
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        DiscordDataContext discordDataContext = scope.ServiceProvider.GetRequiredService<DiscordDataContext>();

        Channel? channel = discordDataContext.Channels.FirstOrDefault(x => x.DiscordChannelFlags.HasFlag(DiscordChannelFlags.AutoAnnouncement));
        if (channel is null)
        {
            Logger.LogError("AutoAnnouncement channel not found");
            return;
        }

        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        SocketTextChannel? discordChannel = guild.GetTextChannel(channel.DiscordId);
        await discordChannel.SendMessageAsync(arg).ConfigureAwait(false);
    }
}