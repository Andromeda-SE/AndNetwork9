using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class ReadMemberByIdSender : BrokerSenderWithResponse<int, Member?>
{
    public const string QUEUE_NAME = "And9.Service.Core.Member.ReadById";
    public ReadMemberByIdSender(BrokerManager brokerManager) : base(brokerManager) { }
}