using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Senders;

public class RegisterChannelCategorySender : BaseRabbitSenderWithResponse<ulong, bool>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.RegisterChannelCategory";

    public RegisterChannelCategorySender(IConnection connection, ILogger<RegisterChannelCategorySender> logger)
        : base(connection, QUEUE_NAME, logger) { }
}