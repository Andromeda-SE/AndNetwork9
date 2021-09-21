using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.VK
{
    public class WallPublishSender:BaseRabbitSenderWithoutResponse<string>
    {
        public const string QUEUE_NAME = "Vk.WallPublish";
        protected WallPublishSender(IConnection connection, ILogger<BaseRabbitSenderWithoutResponse<string>> logger) : base(connection, QUEUE_NAME, logger)
        {
        }
    }
}