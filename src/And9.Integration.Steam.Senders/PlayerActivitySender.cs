using And9.Integration.Steam.Senders.Models;
using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Steam.Senders;

public class PlayerActivitySender : BaseRabbitSenderWithResponse<ulong[], PlayerActivityResultNode[]>
{
    public const string QUEUE_NAME = "And9.Integration.Steam.PlayerActivity";

    public PlayerActivitySender(IConnection connection,
        ILogger<BaseRabbitSenderWithResponse<ulong[], PlayerActivityResultNode[]>> logger) : base(connection,
        QUEUE_NAME,
        logger) { }
}