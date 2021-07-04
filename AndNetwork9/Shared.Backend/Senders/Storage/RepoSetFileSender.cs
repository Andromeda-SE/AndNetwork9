using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Storage;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Storage
{
    public class RepoSetFileSender : BaseRabbitSenderWithoutResponse<RepoNodeWithData>
    {
        public const string QUEUE_NAME = "Storage.RepoSetFile";

        public RepoSetFileSender(IConnection connection) : base(connection, QUEUE_NAME) { }
    }
}