using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Integration.Steam.Senders;

public class ResolveSteamUrlSender : BaseRabbitSenderWithResponse<string, ulong?>
{
    public const string QUEUE_NAME = "And9.Integration.Steam.ResolveSteamUrl";

    public ResolveSteamUrlSender(IConnection connection,
        ILogger<BaseRabbitSenderWithResponse<string, ulong?>> logger) : base(connection, QUEUE_NAME, logger) { }
}