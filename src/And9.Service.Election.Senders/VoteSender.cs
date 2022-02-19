using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Election.Senders;

public class VoteSender : BaseRabbitSenderWithResponse<int, bool>
{
    public const string QUEUE_NAME = "And9.Service.Election.Vote";
    public VoteSender(IConnection connection, ILogger<VoteSender> logger) : base(connection, QUEUE_NAME, logger) { }
}