using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Discord;

public class PublishSender : BaseRabbitSenderWithoutResponse<string>
{
    public const string QUEUE_NAME = "Discord.Publish";

    public PublishSender(IConnection connection, ILogger<PublishSender> logger) : base(connection,
        QUEUE_NAME,
        logger) { }
}