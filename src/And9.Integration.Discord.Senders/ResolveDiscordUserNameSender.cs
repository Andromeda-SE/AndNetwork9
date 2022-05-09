using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Integration.Discord.Senders;

[QueueName(QUEUE_NAME)]
public class ResolveDiscordUserNameSender : BrokerSenderWithResponse<string, ulong?>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.ResolveDiscordUserName";
    public ResolveDiscordUserNameSender(BrokerManager brokerManager) : base(brokerManager) { }
}