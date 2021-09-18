using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Discord
{
    public class SendSender : BaseRabbitSenderWithoutResponse<SendArg>
    {
        public const string QUEUE_NAME = "Discord.Send";

        public SendSender(IConnection connection, ILogger<SendSender> logger) : base(connection, QUEUE_NAME, logger) { }
    }
}