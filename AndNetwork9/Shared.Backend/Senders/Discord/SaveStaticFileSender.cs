using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Storage;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Discord
{
    public class SaveStaticFileSender : BaseRabbitSenderWithResponse<SaveStaticFileArg, StaticFile>
    {
        public const string QUEUE_NAME = "Discord.SaveStaticFile";
        public SaveStaticFileSender(IConnection connection) : base(connection, QUEUE_NAME) { }
    }
}