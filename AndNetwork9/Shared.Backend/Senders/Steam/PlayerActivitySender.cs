using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Elections;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Steam
{
    public class PlayerActivitySender : BaseRabbitSenderWithResponse<ulong[], PlayerActivityResultNode[]>
    {
        public const string QUEUE_NAME = "Steam.PlayerActivity";

        protected PlayerActivitySender(IConnection connection, string queue, ILogger<BaseRabbitSenderWithResponse<ulong[], PlayerActivityResultNode[]>> logger) : base(connection, queue, logger) { }
    }
}