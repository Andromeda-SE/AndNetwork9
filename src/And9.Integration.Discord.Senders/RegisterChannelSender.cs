using And9.Integration.Discord.Abstractions.Interfaces;
using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Senders;

public class RegisterChannelSender : BaseRabbitSenderWithResponse<IChannel, bool>
{
    public const string QUEUE_NAME = "And9.Integration.Discord.RegisterChannel";

    public RegisterChannelSender(IConnection connection, ILogger<RegisterChannelSender> logger)
        : base(connection, QUEUE_NAME, logger) { }
}