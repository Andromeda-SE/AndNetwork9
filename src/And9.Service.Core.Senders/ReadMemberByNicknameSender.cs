using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class ReadMemberByNicknameSender : BrokerSenderWithResponse<string, Member?>
{
    public const string QUEUE_NAME = "And9.Service.Core.Member.ReadByNickname";
    public ReadMemberByNicknameSender(BrokerManager brokerManager) : base(brokerManager) { }
}