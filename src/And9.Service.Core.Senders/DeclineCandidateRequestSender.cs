using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Core.Senders;

public class DeclineCandidateRequestSender : BaseRabbitSenderWithoutResponse<int>
{
    public const string QUEUE_NAME = "And9.Service.Core.DeclineCandidateRequest";
    public DeclineCandidateRequestSender(IConnection connection, ILogger<DeclineCandidateRequestSender> logger) : base(connection, QUEUE_NAME, logger) { }
}