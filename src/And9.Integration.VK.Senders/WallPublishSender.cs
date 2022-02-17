using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.VK.Senders;

public class WallPublishSender : BaseRabbitSenderWithoutResponse<string>
{
    public const string QUEUE_NAME = "And9.Integration.Vk.WallPublish";

    protected WallPublishSender(IConnection connection, ILogger<BaseRabbitSenderWithoutResponse<string>> logger) :
        base(connection, QUEUE_NAME, logger) { }
}