using And9.Integration.Discord.Senders.Models;
using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Integration.Discord.Senders;

[QueueName(QUEUE_NAME)]
public class SendDirectMessageSender : BrokerSenderWithoutResponse<SendDirectMessageArg>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.SendDirectMessage";
    public SendDirectMessageSender(BrokerManager brokerManager) : base(brokerManager) { }
}