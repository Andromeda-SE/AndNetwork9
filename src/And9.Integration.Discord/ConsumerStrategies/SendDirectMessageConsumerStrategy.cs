using System.Net;
using And9.Integration.Discord.Senders;
using And9.Integration.Discord.Senders.Models;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using Discord;
using Discord.Rest;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(SendDirectMessageSender.QUEUE_NAME)]
public class SendDirectMessageConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<SendDirectMessageArg>
{
    private readonly DiscordBot _bot;

    public SendDirectMessageConsumerStrategy(DiscordBot bot) => _bot = bot;

    public async ValueTask ExecuteAsync(SendDirectMessageArg arg)
    {
        (ulong discordId, string? message) = arg;
        RestUser? user = await _bot.Rest.GetUserAsync(discordId).ConfigureAwait(false);
        if (user is null) throw new(HttpStatusCode.NotFound.ToString());
        await user.SendMessageAsync(message).ConfigureAwait(false);
    }
}