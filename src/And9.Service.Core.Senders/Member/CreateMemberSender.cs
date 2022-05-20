using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Member;

[QueueName(QUEUE_NAME)]
public class CreateMemberSender : BrokerSenderWithResponse<Abstractions.Models.Member, int>
{
    public const string QUEUE_NAME = "And9.Service.Core.Member.Create";
    public CreateMemberSender(BrokerManager brokerManager) : base(brokerManager) { }
}