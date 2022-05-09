using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Service.Election.Senders;

[QueueName(QUEUE_NAME)]
public class VoteSender : BrokerSenderWithResponse<(int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes), bool>
{
    public const string QUEUE_NAME = "And9.Service.Election.Vote";
    public VoteSender(BrokerManager brokerManager) : base(brokerManager) { }
}