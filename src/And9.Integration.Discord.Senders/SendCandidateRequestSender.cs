using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Interfaces;

namespace And9.Integration.Discord.Senders;

[QueueName(QUEUE_NAME)]
public class SendCandidateRequestSender : BrokerSenderWithoutResponse<ICandidateRequest>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.SendCandidateRequest";
    public SendCandidateRequestSender(BrokerManager brokerManager) : base(brokerManager) { }
}