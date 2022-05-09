using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Integration.Discord.Senders;

[QueueName(QUEUE_NAME)]
public class RegisterChannelCategorySender : BrokerSenderWithResponse<ulong, bool>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.RegisterChannelCategory";

    public RegisterChannelCategorySender(BrokerManager brokerManager) : base(brokerManager) { }
}