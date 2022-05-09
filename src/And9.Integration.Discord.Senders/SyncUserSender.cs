using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Models;

namespace And9.Integration.Discord.Senders;

[QueueName(QUEUE_NAME)]
public class SyncUserSender : BrokerSenderWithoutResponse<Member>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.UpdateUser";
    public SyncUserSender(BrokerManager brokerManager) : base(brokerManager) { }
}