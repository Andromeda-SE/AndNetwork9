using System;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Enums;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Senders.Elections
{
    public class NextStageSender : BaseRabbitSenderWithoutResponse<ElectionStage>
    {
        public const string QUEUE_NAME = "Elections.NextStage";

        public NextStageSender(IConnection connection, ILogger<NextStageSender> logger) : base(connection, QUEUE_NAME, logger) { }
    }
}