using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Interfaces;

namespace And9.Service.Core.Senders;

public class ReadSquadSender : BrokerSenderWithResponse<short, ISquad>
{
    public const string QUEUE_NAME = "And9.Service.Core.ReadSquad";
    public ReadSquadSender(BrokerManager brokerManager) : base(brokerManager) { }
}