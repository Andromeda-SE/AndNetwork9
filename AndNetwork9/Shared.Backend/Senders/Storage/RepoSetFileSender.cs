using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Storage;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Storage
{
    public class RepoSetFileSender : BaseRabbitSenderWithoutResponse<RepoNodeWithData>
    {
        public const string QUEUE_NAME = "Storage.RepoSetFile";

        public RepoSetFileSender(IConnection connection, ILogger<RepoSetFileSender> logger) : base(connection,
            QUEUE_NAME,
            logger) { }
    }
}