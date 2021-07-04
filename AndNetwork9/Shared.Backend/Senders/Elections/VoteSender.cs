using AndNetwork9.Shared.Backend.Rabbit;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Elections
{
    public class VoteSender : BaseRabbitSenderWithoutResponse<VoteArg>
    {
        public const string QUEUE_NAME = "Elections.Vote";

        public VoteSender(IConnection connection) : base(connection, QUEUE_NAME) { }
    }
}