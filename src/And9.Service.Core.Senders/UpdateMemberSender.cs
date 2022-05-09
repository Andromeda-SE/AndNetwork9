using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class UpdateMemberSender : BrokerSenderWithResponse<Member, Member>
{
    public const string QUEUE_NAME = "And9.Service.Core.Member.Update";
    public UpdateMemberSender(BrokerManager brokerManager) : base(brokerManager) { }
}