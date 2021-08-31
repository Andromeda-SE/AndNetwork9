using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Elections
{
    public class VoteSender : BaseRabbitSenderWithoutResponse<VoteArg>
    {
        public const string QUEUE_NAME = "Elections.Vote";

        public VoteSender(IConnection connection, ILogger<VoteSender> logger) : base(connection, QUEUE_NAME, logger) { }
    }
}