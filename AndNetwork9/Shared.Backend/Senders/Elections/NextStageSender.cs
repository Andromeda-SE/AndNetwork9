using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Enums;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Elections
{
    public class NextStageSender : BaseRabbitSenderWithoutResponse<ElectionStage>
    {
        public const string QUEUE_NAME = "Elections.NextStage";

        public NextStageSender(IConnection connection) : base(connection, QUEUE_NAME) { }
    }
}