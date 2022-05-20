using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Member;

[QueueName(QUEUE_NAME)]
public class ReadMemberByNicknameSender : BrokerSenderWithResponse<string, Abstractions.Models.Member?>
{
    public const string QUEUE_NAME = "And9.Service.Core.Member.ReadByNickname";
    public ReadMemberByNicknameSender(BrokerManager brokerManager) : base(brokerManager) { }
}