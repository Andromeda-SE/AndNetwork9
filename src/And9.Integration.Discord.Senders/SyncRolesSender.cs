using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Senders;

public class SyncRolesSender : BaseRabbitSenderWithoutResponse<object>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.SyncRolesSender";

    public SyncRolesSender(IConnection connection, ILogger<SyncRolesSender> logger) : base(
        connection,
        QUEUE_NAME,
        logger) { }
}