using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Member;

[QueueName(QUEUE_NAME)]
public class ReadMemberByDiscordIdSender : BrokerSenderWithResponse<ulong, Abstractions.Models.Member?>
{
    public const string QUEUE_NAME = "And9.Service.Core.Member.ReadByDiscordId";
    public ReadMemberByDiscordIdSender(BrokerManager brokerManager) : base(brokerManager) { }
}