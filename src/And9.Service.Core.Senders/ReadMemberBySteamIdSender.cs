using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Core.Senders;

public class ReadMemberBySteamIdSender : BaseRabbitSenderWithResponse<ulong, Member?>
{
    public const string QUEUE_NAME = MemberCrudSender.QUEUE_NAME + "." + nameof(IMemberModelServiceMethods.ReadBySteamId);
    internal ReadMemberBySteamIdSender(IConnection connection, ILogger<BaseRabbitSenderWithResponse<ulong, Member?>> logger) : base(connection, QUEUE_NAME, logger) { }
}