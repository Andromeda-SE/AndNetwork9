using And9.Gateway.Clan.Senders.Models;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Gateway.Clan.Senders;

public class RaiseMemberUpdateSender : BaseRabbitSenderWithResponse<RaiseMemberUpdateArg, Rank>
{
    public const string QUEUE_NAME = "And9.Gateway.Clan.RaiseMemberUpdate";

    public RaiseMemberUpdateSender(IConnection connection, ILogger<BaseRabbitSenderWithResponse<RaiseMemberUpdateArg, Rank>> logger)
        : base(connection, QUEUE_NAME, logger) { }
}