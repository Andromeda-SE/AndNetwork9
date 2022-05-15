using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders;

public class DisbandSquadSender : BrokerSenderWithoutResponse<short>
{
    public const string QUEUE_NAME = "And9.Service.Core.DisbandSquad";
    public DisbandSquadSender(BrokerManager brokerManager) : base(brokerManager) { }
}