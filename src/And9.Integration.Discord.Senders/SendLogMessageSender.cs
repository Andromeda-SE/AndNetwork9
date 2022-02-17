using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Senders;

public class SendLogMessageSender : BaseRabbitSenderWithoutResponse<string>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.SendLogMessage";

    public SendLogMessageSender(IConnection connection, ILogger<SendLogMessageSender> logger) : base(connection,
        QUEUE_NAME,
        logger) { }
}