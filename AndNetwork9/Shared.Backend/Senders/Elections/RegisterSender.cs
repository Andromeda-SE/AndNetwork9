using AndNetwork9.Shared.Backend.Rabbit;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Elections
{
    public class RegisterSender : BaseRabbitSenderWithoutResponse<int>
    {
        public const string QUEUE_NAME = "Elections.Register";

        public RegisterSender(IConnection connection) : base(connection, QUEUE_NAME) { }
    }
}