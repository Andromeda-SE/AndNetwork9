using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Interfaces;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class ReadSquadMembershipHistorySender : BrokerSenderWithCollectionResponse<short, ISquadMembershipHistoryEntry>
{
    public const string QUEUE_NAME = "And9.Service.Core.ReadSquadMembershipHistory";
    public ReadSquadMembershipHistorySender(BrokerManager brokerManager) : base(brokerManager) { }
}