using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Steam;

public class PlayerActivitySender : BaseRabbitSenderWithResponse<ulong[], PlayerActivityResultNode[]>
{
    public const string QUEUE_NAME = "Steam.PlayerActivity";

    public PlayerActivitySender(IConnection connection,
        ILogger<BaseRabbitSenderWithResponse<ulong[], PlayerActivityResultNode[]>> logger) : base(connection,
        QUEUE_NAME,
        logger) { }
}