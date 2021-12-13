using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Discord;

public class ResolveDiscordUserNameSender : BaseRabbitSenderWithResponse<string, ulong?>
{
    public const string QUEUE_NAME = "Discord.ResolveDiscordUserName";

    public ResolveDiscordUserNameSender(IConnection connection, ILogger<ResolveDiscordUserNameSender> logger) : base(
        connection,
        QUEUE_NAME,
        logger) { }
}