using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Election.Senders;

[QueueName(QUEUE_NAME)]
public class RegisterSender : BrokerSenderWithResponse<int, bool>
{
    public const string QUEUE_NAME = "And9.Service.Election.Register";
    public RegisterSender(BrokerManager brokerManager) : base(brokerManager) { }
}