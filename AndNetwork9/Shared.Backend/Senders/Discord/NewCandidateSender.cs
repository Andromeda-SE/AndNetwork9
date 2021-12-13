using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Utility;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Discord;

public class NewCandidateSender : BaseRabbitSenderWithoutResponse<CandidateRequest>
{
    public const string QUEUE_NAME = "Discord.NewCandidate";

    public NewCandidateSender(IConnection connection, ILogger<NewCandidateSender> logger) : base(connection,
        QUEUE_NAME,
        logger) { }
}