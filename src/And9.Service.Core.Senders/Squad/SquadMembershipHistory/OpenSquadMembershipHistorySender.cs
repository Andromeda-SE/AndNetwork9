using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Squad.SquadMembershipHistory;

[QueueName(QUEUE_NAME)]
public class OpenSquadMembershipHistorySender : BrokerSenderWithoutResponse<(int memberId, short squadNumber)>
{
    public const string QUEUE_NAME = "And9.Service.Core.OpenSquadMembershipHistory";
    public OpenSquadMembershipHistorySender(BrokerManager brokerManager) : base(brokerManager) { }
}