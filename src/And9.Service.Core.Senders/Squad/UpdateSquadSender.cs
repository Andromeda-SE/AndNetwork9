using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Squad;

[QueueName(QUEUE_NAME)]
public class UpdateSquadSender : BrokerSenderWithResponse<Abstractions.Models.Squad, Abstractions.Models.Squad>
{
    public const string QUEUE_NAME = "And9.Service.Core.Squad.Update";
    public UpdateSquadSender(BrokerManager brokerManager) : base(brokerManager) { }
}