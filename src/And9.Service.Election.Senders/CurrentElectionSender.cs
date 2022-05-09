using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Election.Senders;

[QueueName(QUEUE_NAME)]
public class CurrentElectionSender : BrokerSenderWithCollectionResponse<int, Abstractions.Models.Election>
{
    public const string QUEUE_NAME = "And9.Service.Election.CurrentElection";
    public CurrentElectionSender(BrokerManager brokerManager) : base(brokerManager) { }
}