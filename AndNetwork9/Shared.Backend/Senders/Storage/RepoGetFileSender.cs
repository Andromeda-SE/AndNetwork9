using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Storage;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Storage
{
    public class RepoGetFileSender : BaseRabbitSenderWithResponse<RepoNode, byte[]>
    {
        public const string QUEUE_NAME = "Storage.RepoGetFile";

        public RepoGetFileSender(IConnection connection) : base(connection, QUEUE_NAME) { }
    }
}