using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Integration.VK.Senders;

[QueueName(QUEUE_NAME)]
public class ResolveVkUrlSender : BrokerSenderWithResponse<string, long?>
{
    public const string QUEUE_NAME = "And9.Integration.Vk.ResolveVkUrl";
    public ResolveVkUrlSender(BrokerManager brokerManager) : base(brokerManager) { }
}