using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Squad.SquadRequest;

[QueueName(QUEUE_NAME)]
public class ReadMemberSquadRequestSender : BrokerSenderWithCollectionResponse<int, Abstractions.Models.SquadRequest>
{
    public const string QUEUE_NAME = "And9.Service.Core.ReadMemberSquadRequest";
    public ReadMemberSquadRequestSender(BrokerManager brokerManager) : base(brokerManager) { }
}