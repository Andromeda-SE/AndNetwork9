using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.Senders.CandidateRequest;

[QueueName(QUEUE_NAME)]
public class ReadCandidateRequestSender : BrokerSenderWithResponse<int, CandidateRegisteredRequest?>
{
    public const string QUEUE_NAME = "And9.Service.Core.CandidateRequest.Read";
    public ReadCandidateRequestSender(BrokerManager brokerManager) : base(brokerManager) { }
}