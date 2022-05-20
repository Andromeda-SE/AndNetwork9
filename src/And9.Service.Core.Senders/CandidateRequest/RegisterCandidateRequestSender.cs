using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.CandidateRequest;

[QueueName(QUEUE_NAME)]
public class RegisterCandidateRequestSender : BrokerSenderWithResponse<Abstractions.Models.CandidateRequest, int>
{
    public const string QUEUE_NAME = "And9.Service.Core.RegisterCandidateRequest";
    public RegisterCandidateRequestSender(BrokerManager brokerManager) : base(brokerManager) { }
}