using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Integration.Discord.Senders;

[QueueName(QUEUE_NAME)]
public class SyncRolesSender : BrokerSenderWithoutResponse<object>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.SyncRolesSender";
    public SyncRolesSender(BrokerManager brokerManager) : base(brokerManager) { }
}