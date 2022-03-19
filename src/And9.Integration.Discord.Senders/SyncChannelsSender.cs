using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Senders;

public class SyncChannelsSender : BaseRabbitSenderWithoutResponse<object>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.SyncChannelsSender";

    public SyncChannelsSender(IConnection connection, ILogger<SyncChannelsSender> logger) : base(
        connection,
        QUEUE_NAME,
        logger) { }
}