using AndNetwork9.Shared.Backend.Elections;
using AndNetwork9.Shared.Backend.Rabbit;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Discord
{
    public class RewriteElectionsChannelSender : BaseRabbitSenderWithoutResponse<Election>
    {
        public const string QUEUE_NAME = "Discord.RewriteElectionsChannel";
        public RewriteElectionsChannelSender(IConnection connection) : base(connection, QUEUE_NAME) { }
    }
}