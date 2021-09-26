using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
namespace AndNetwork9.Shared.Backend.Senders.VK
{
    public class ResolveVkUrlSender : BaseRabbitSenderWithResponse<string, long?>
    {
        public const string QUEUE_NAME = "Vk.ResolveVkUrl";
        protected ResolveVkUrlSender(IConnection connection, ILogger<BaseRabbitSenderWithResponse<string, long?>> logger) : base(connection, QUEUE_NAME, logger)
        {
        }
    }
}