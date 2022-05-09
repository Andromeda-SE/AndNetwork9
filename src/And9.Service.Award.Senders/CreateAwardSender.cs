using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Award.Senders;

[QueueName(QUEUE_NAME)]
public class CreateAwardSender : BrokerSenderWithResponse<Abstractions.Models.Award, int>
{
    public const string QUEUE_NAME = "And9.Service.Award.Award.Create";
    public CreateAwardSender(BrokerManager brokerManager) : base(brokerManager) { }
}