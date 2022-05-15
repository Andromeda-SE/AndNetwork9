using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class CreateSquadPartSender : BrokerSenderWithoutResponse<(short squadNumber, int leaderId)>
{
    public const string QUEUE_NAME = "And9.Service.Core.CreateSquadPart";
    public CreateSquadPartSender(BrokerManager brokerManager) : base(brokerManager) { }
}