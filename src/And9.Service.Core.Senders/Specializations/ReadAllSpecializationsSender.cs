using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.Senders.Specializations;

[QueueName(QUEUE_NAME)]
public class ReadAllSpecializationsSender : BrokerSenderWithCollectionResponse<int, Specialization>
{
    public const string QUEUE_NAME = "And9.Service.Core.Specialization.ReadAll";
    public ReadAllSpecializationsSender(BrokerManager brokerManager) : base(brokerManager) { }
}