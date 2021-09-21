using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Storage;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Storage
{
    public class NewRepoSender : BaseRabbitSenderWithResponse<Repo, Repo>
    {
        public const string QUEUE_NAME = "Storage.NewRepo";

        public NewRepoSender(IConnection connection, ILogger<NewRepoSender> logger) : base(connection,
            QUEUE_NAME,
            logger) { }
    }
}