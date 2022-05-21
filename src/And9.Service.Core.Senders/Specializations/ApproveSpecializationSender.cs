using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Specializations;

[QueueName(QUEUE_NAME)]
public class ApproveSpecializationSender : BrokerSenderWithoutResponse<(int memberId, int specialzationId)>
{
    public const string QUEUE_NAME = "And9.Service.Core.Specialization.Approve";
    public ApproveSpecializationSender(BrokerManager brokerManager) : base(brokerManager) { }
}