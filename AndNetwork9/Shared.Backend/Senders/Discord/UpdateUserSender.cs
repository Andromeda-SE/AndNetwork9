using AndNetwork9.Shared.Backend.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Discord;

public class UpdateUserSender : BaseRabbitSenderWithoutResponse<ulong>
{
    public const string QUEUE_NAME = "Discord.UpdateUser";

    public UpdateUserSender(IConnection connection, ILogger<UpdateUserSender> logger) : base(connection,
        QUEUE_NAME,
        logger) { }
}