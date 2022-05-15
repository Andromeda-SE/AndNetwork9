using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Interfaces;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class ReadMemberSquadMembershipHistorySender : BrokerSenderWithCollectionResponse<int, ISquadMembershipHistoryEntry>
{
    public const string QUEUE_NAME = "And9.Service.Core.ReadMemberSquadMembershipHistory";
    public ReadMemberSquadMembershipHistorySender(BrokerManager brokerManager) : base(brokerManager) { }
}