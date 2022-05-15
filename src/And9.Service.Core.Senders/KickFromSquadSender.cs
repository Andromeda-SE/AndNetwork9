using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders;

public class KickFromSquadSender : BrokerSenderWithoutResponse<int>
{
    public const string QUEUE_NAME = "And9.Service.Core.KickFromSquad";
    public KickFromSquadSender(BrokerManager brokerManager) : base(brokerManager) { }
}