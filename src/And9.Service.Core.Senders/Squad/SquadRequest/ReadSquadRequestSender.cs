using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Squad.SquadRequest;

[QueueName(QUEUE_NAME)]
public class ReadSquadRequestSender : BrokerSenderWithCollectionResponse<short, Abstractions.Models.SquadRequest>
{
    public const string QUEUE_NAME = "And9.Service.Core.ReadSquadRequest";
    public ReadSquadRequestSender(BrokerManager brokerManager) : base(brokerManager) { }
}