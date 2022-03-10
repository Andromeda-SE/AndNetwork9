using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Election.Senders;

public class VoteSender : BaseRabbitSenderWithResponse<(int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes), bool>
{
    public const string QUEUE_NAME = "And9.Service.Election.Vote";
    public VoteSender(IConnection connection, ILogger<VoteSender> logger) : base(connection, QUEUE_NAME, logger) { }
}