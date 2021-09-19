using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Steam
{
    public class ResolveSteamUrlSender : BaseRabbitSenderWithResponse<string, ulong?>
    {
        public const string QUEUE_NAME = "Steam.ResolveSteamUrl";

        protected ResolveSteamUrlSender(IConnection connection, string queue, ILogger<BaseRabbitSenderWithResponse<string, ulong?>> logger) : base(connection, queue, logger) { }

    }
}