using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Election.Senders;

public class CurrentElectionSender : BaseRabbitSenderWithStreamResponse<int, Abstractions.Models.Election>
{
    public const string QUEUE_NAME = "And9.Service.Election.CurrentElection";
    protected CurrentElectionSender(IConnection connection, ILogger<CurrentElectionSender> logger) : base(connection, QUEUE_NAME, logger) { }
}