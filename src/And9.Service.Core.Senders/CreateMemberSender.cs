using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class CreateMemberSender : BrokerSenderWithResponse<Member, int>
{
    public const string QUEUE_NAME = "And9.Service.Core.Member.Create";
    public CreateMemberSender(BrokerManager brokerManager) : base(brokerManager) { }
}