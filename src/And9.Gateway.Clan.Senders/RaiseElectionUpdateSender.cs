using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Gateway.Clan.Senders;

[QueueName(QUEUE_NAME)]
public class RaiseElectionUpdateSender : BrokerSenderWithoutResponse<int>
{
    public const string QUEUE_NAME = "And9.Gateway.Clan.RaiseElectionUpdate";
    public RaiseElectionUpdateSender(BrokerManager brokerManager) : base(brokerManager) { }
}