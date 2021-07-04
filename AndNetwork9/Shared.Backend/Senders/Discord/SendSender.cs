using AndNetwork9.Shared.Backend.Rabbit;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Discord
{
    public class SendSender : BaseRabbitSenderWithoutResponse<SendArg>
    {
        public const string QUEUE_NAME = "Discord.Send";

        public SendSender(IConnection connection) : base(connection, QUEUE_NAME) { }
    }
}