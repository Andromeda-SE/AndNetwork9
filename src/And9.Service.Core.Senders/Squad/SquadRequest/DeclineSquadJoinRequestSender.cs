using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Squad.SquadRequest;

[QueueName(QUEUE_NAME)]
public class DeclineSquadJoinRequestSender : BrokerSenderWithoutResponse<(short number, int memberId, bool byMember)>
{
    public const string QUEUE_NAME = "And9.Service.Core.DeclineSquadJoinRequest";
    public DeclineSquadJoinRequestSender(BrokerManager brokerManager) : base(brokerManager) { }
}