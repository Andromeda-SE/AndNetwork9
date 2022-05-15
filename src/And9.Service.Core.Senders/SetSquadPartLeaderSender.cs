using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class SetSquadPartLeaderSender : BrokerSenderWithoutResponse<(int memberId, short squadNumber, short squadPartNumber)>
{
    public const string QUEUE_NAME = "And9.Service.Core.SetSquadPartLeader";
    public SetSquadPartLeaderSender(BrokerManager brokerManager) : base(brokerManager) { }
}