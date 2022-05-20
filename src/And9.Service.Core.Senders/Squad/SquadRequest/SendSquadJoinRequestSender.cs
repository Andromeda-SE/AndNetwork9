using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Squad.SquadRequest;

[QueueName(QUEUE_NAME)]
public class SendSquadJoinRequestSender : BrokerSenderWithoutResponse<(int memberId, short squadNumber)>
{
    public const string QUEUE_NAME = "And9.Service.Core.SendSquadJoinRequest";
    public SendSquadJoinRequestSender(BrokerManager brokerManager) : base(brokerManager) { }
}