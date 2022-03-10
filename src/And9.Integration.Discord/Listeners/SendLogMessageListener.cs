using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Database.Models;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using Discord.WebSocket;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Listeners;

public class SendLogMessageListener : BaseRabbitListenerWithoutResponse<string>
{
    private readonly DiscordBot _bot;
    private readonly DiscordDataContext _discordDataContext;

    public SendLogMessageListener(IConnection connection, DiscordBot bot, ILogger<SendLogMessageListener> logger, DiscordDataContext discordDataContext)
        : base(connection, SendDirectMessageSender.QUEUE_NAME, logger)
    {
        _bot = bot;
        _discordDataContext = discordDataContext;
    }

    public override async Task Run(string arg)
    {
        Channel? channel = _discordDataContext.Channels.FirstOrDefault(x => x.DiscordChannelFlags.HasFlag(DiscordChannelFlags.AutoAnnouncement));
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