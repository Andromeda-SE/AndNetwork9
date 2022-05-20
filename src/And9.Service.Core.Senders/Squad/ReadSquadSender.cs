using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders.Squad;

public class ReadSquadSender : BrokerSenderWithResponse<int, Abstractions.Models.Squad>
{
    public const string QUEUE_NAME = "And9.Service.Core.ReadSquad";
    public ReadSquadSender(BrokerManager brokerManager) : base(brokerManager) { }
}