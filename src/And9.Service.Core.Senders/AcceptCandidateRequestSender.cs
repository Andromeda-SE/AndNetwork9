using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Core.Senders;

public class AcceptCandidateRequestSender : BaseRabbitSenderWithoutResponse<int>
{
    public const string QUEUE_NAME = "And9.Service.Core.AcceptCandidateRequest";
    public AcceptCandidateRequestSender(IConnection connection, ILogger<AcceptCandidateRequestSender> logger) : base(connection, QUEUE_NAME, logger) { }
}