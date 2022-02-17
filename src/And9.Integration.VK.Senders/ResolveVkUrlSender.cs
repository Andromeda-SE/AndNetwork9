using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.VK.Senders;

public class ResolveVkUrlSender : BaseRabbitSenderWithResponse<string, long?>
{
    public const string QUEUE_NAME = "And9.Integration.Vk.ResolveVkUrl";

    public ResolveVkUrlSender(IConnection connection,
        ILogger<BaseRabbitSenderWithResponse<string, long?>> logger) : base(connection, QUEUE_NAME, logger) { }
}