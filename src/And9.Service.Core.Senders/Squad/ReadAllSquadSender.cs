using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Interfaces;

namespace And9.Service.Core.Senders.Squad;

[QueueName(QUEUE_NAME)]
public class ReadAllSquadSender : BrokerSenderWithCollectionResponse<int, ISquad>
{
    public const string QUEUE_NAME = "And9.Service.Core.ReadAllSquad";
    public ReadAllSquadSender(BrokerManager brokerManager) : base(brokerManager) { }
}