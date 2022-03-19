using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Core.Senders;

public class ReadMemberByNicknameSender : BaseRabbitSenderWithResponse<string, Member?>
{
    public const string QUEUE_NAME = MemberCrudSender.QUEUE_NAME + "." + nameof(IMemberModelServiceMethods.ReadByNickname);
    internal ReadMemberByNicknameSender(IConnection connection, ILogger<ReadMemberByNicknameSender> logger) : base(connection, QUEUE_NAME, logger) { }
}