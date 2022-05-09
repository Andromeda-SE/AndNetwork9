using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Award.Senders;

[QueueName(QUEUE_NAME)]
public class ReadByMemberIdAwardSender : BrokerSenderWithCollectionResponse<int, Abstractions.Models.Award>
{
    public const string QUEUE_NAME = "And9.Service.Award.Award.ReadByMemberId";
    public ReadByMemberIdAwardSender(BrokerManager brokerManager) : base(brokerManager) { }
}