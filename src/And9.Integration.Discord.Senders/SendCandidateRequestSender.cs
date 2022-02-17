using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Senders;

public class SendCandidateRequestSender : BaseRabbitSenderWithoutResponse<ICandidateRequest>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.SendCandidateRequest";

    public SendCandidateRequestSender(IConnection connection, ILogger<SendCandidateRequestSender> logger) : base(connection,
        QUEUE_NAME,
        logger) { }
}