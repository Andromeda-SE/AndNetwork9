using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Senders;

public class SyncUserSender : BaseRabbitSenderWithoutResponse<Member>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.UpdateUser";

    public SyncUserSender(IConnection connection, ILogger<SyncUserSender> logger) : base(connection,
        QUEUE_NAME,
        logger) { }
}