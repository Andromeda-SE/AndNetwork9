using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Auth.Senders;

[QueueName(QUEUE_NAME)]
public class GeneratePasswordSender : BrokerSenderWithResponse<int, string>
{
    public const string QUEUE_NAME = "And9.Service.Auth.GeneratePassword";
    public GeneratePasswordSender(BrokerManager brokerManager) : base(brokerManager) { }
}