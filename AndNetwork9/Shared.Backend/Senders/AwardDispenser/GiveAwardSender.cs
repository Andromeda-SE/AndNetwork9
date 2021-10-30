using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.AwardDispenser;

public class GiveAwardSender : BaseRabbitSenderWithoutResponse<Award>
{
    public const string QUEUE_NAME = "AwardDispenser.GiveAward";

    public GiveAwardSender(IConnection connection, ILogger<BaseRabbitSenderWithoutResponse<Award>> logger) : base(
        connection,
        QUEUE_NAME,
        logger) { }
}