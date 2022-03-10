using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Gateway.Clan.Senders;

public class RaiseElectionUpdateSender : BaseRabbitSenderWithoutResponse<int>
{
    public const string QUEUE_NAME = "And9.Gateway.Clan.RaiseElectionUpdate";

    public RaiseElectionUpdateSender(IConnection connection, ILogger<BaseRabbitSenderWithoutResponse<int>> logger)
        : base(connection, QUEUE_NAME, logger) { }
}