using And9.Integration.Discord.Abstractions.Interfaces;
using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Integration.Discord.Senders;

[QueueName(QUEUE_NAME)]
public class CreateChannelSender : BrokerSenderWithResponse<IChannel, ulong>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.CreateChannel";
    public CreateChannelSender(BrokerManager brokerManager) : base(brokerManager) { }
}