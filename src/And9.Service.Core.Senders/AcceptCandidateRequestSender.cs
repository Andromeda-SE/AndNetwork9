using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class AcceptCandidateRequestSender : BrokerSenderWithoutResponse<int>
{
    public const string QUEUE_NAME = "And9.Service.Core.CandidateRequest.Accept";

    public AcceptCandidateRequestSender(BrokerManager brokerManager) : base(brokerManager) { }
}