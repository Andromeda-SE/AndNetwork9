using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders;

[QueueName(QUEUE_NAME)]
public class AppendSquadNameSender : BrokerSenderWithoutResponse<(short number, string name)>
{
    public const string QUEUE_NAME = "And9.Service.Core.AppendSquadName";
    public AppendSquadNameSender(BrokerManager brokerManager) : base(brokerManager) { }
}