using AndNetwork9.Shared.Backend.Rabbit;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Discord
{
    public class PublishSender : BaseRabbitSenderWithoutResponse<string>
    {
        public const string QUEUE_NAME = "Discord.Publish";

        public PublishSender(IConnection connection) : base(connection, QUEUE_NAME) { }
    }
}