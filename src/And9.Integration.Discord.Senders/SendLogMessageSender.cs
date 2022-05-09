using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Integration.Discord.Senders;

[QueueName(QUEUE_NAME)]
public class SendLogMessageSender : BrokerSenderWithoutResponse<string>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.SendLogMessage";
    public SendLogMessageSender(BrokerManager brokerManager) : base(brokerManager) { }
}