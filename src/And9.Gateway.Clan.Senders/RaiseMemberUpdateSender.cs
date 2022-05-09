using And9.Gateway.Clan.Senders.Models;
using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Gateway.Clan.Senders;

[QueueName(QUEUE_NAME)]
public class RaiseMemberUpdateSender : BrokerSenderWithResponse<RaiseMemberUpdateArg, Rank>
{
    public const string QUEUE_NAME = "And9.Gateway.Clan.RaiseMemberUpdate";

    public RaiseMemberUpdateSender(BrokerManager brokerManager) : base(brokerManager) { }
}