using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Elections;

public class RegisterSender : BaseRabbitSenderWithoutResponse<int>
{
    public const string QUEUE_NAME = "Elections.Register";

    public RegisterSender(IConnection connection, ILogger<RegisterSender> logger) : base(connection,
        QUEUE_NAME,
        logger) { }
}