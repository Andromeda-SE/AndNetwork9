using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Squad.SquadMembershipHistory;

[QueueName(QUEUE_NAME)]
public class CloseSquadMembershipHistorySender : BrokerSenderWithoutResponse<int>
{
    public const string QUEUE_NAME = "And9.Service.Core.CloseSquadMembershipHistory";
    public CloseSquadMembershipHistorySender(BrokerManager brokerManager) : base(brokerManager) { }
}