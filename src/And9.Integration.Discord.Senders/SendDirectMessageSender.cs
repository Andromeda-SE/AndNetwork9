using And9.Integration.Discord.Senders.Models;
using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Senders;

public class SendDirectMessageSender : BaseRabbitSenderWithoutResponse<SendDirectMessageArg>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.SendDirectMessage";

    public SendDirectMessageSender(IConnection connection, ILogger<SendDirectMessageSender> logger) : base(connection, QUEUE_NAME, logger) { }
}