using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Senders;

public class ResolveDiscordUserNameSender : BaseRabbitSenderWithResponse<string, ulong?>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.ResolveDiscordUserName";

    public ResolveDiscordUserNameSender(IConnection connection, ILogger<ResolveDiscordUserNameSender> logger) : base(
        connection,
        QUEUE_NAME,
        logger) { }
}