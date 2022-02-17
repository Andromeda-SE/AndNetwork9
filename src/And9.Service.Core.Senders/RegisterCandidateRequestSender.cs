using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Core.Senders;

public class RegisterCandidateRequestSender : BaseRabbitSenderWithResponse<CandidateRequest, int>
{
    public const string QUEUE_NAME = "And9.Service.Core.RegisterCandidateRequest";
    public RegisterCandidateRequestSender(IConnection connection, ILogger<RegisterCandidateRequestSender> logger) : base(connection, QUEUE_NAME, logger) { }
}