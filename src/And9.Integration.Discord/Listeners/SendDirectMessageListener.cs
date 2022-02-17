using System.Net;
using And9.Integration.Discord.Senders;
using And9.Integration.Discord.Senders.Models;
using And9.Lib.Broker;
using Discord;
using Discord.Rest;
using IConnection = RabbitMQ.Client.IConnection;

namespace And9.Integration.Discord.Listeners;

public class SendDirectMessageListener : BaseRabbitListenerWithoutResponse<SendDirectMessageArg>
{
    private readonly DiscordBot _bot;

    public SendDirectMessageListener(IConnection connection, DiscordBot bot, ILogger<SendDirectMessageListener> logger)
        : base(connection, SendDirectMessageSender.QUEUE_NAME, logger) => _bot = bot;

    public override async Task Run(SendDirectMessageArg arg)
    {
        (ulong discordId, string? message) = arg;
        RestUser? user = await _bot.Rest.GetUserAsync(discordId).ConfigureAwait(false);
        if (user is null) throw new(HttpStatusCode.NotFound.ToString());
        await user.SendMessageAsync(message).ConfigureAwait(false);
    }
}