using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class CreateSquadSender : BrokerSenderWithoutResponse<(short? sqauadNumber, int leaderId)>
{
    public const string QUEUE_NAME = "And9.Service.Core.CreateSquad";
    public CreateSquadSender(BrokerManager brokerManager) : base(brokerManager) { }
}