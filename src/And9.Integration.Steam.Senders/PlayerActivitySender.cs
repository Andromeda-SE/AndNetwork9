using And9.Integration.Steam.Senders.Models;
using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Integration.Steam.Senders;

[QueueName(QUEUE_NAME)]
public class PlayerActivitySender : BrokerSenderWithResponse<ulong[], PlayerActivityResultNode[]>
{
    public const string QUEUE_NAME = "And9.Integration.Steam.PlayerActivity";
    public PlayerActivitySender(BrokerManager brokerManager) : base(brokerManager) { }
}