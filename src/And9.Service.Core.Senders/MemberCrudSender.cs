using And9.Lib.Broker;
using And9.Lib.Broker.Crud;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Core.Senders;

public class MemberCrudSender : RabbitCrudSender<Member>, IMemberModelServiceMethods
{
    public const string QUEUE_NAME = "And9.Service.Core.Member";
    public const CrudFlags CRUD_FLAGS = CrudFlags.Create | CrudFlags.Read | CrudFlags.Update;

    private readonly ReadMemberBySteamIdSender _readMemberBySteamIdSender;

    public MemberCrudSender(IConnection connection,
        ILogger<BaseRabbitSenderWithResponse<Member, int>> createLogger,
        ILogger<BaseRabbitSenderWithResponse<int, Member>> readLogger,
        ILogger<BaseRabbitSenderWithStreamResponse<object, Member>> readCollectionLogger,
        ILogger<BaseRabbitSenderWithResponse<Member, Member>> updateLogger,
        ILogger<BaseRabbitSenderWithoutResponse<int>> deleteLogger,
        ILogger<BaseRabbitSenderWithResponse<ulong, Member?>> readBySteamIdLogger)
        : base(connection, QUEUE_NAME, CRUD_FLAGS, createLogger, readLogger, readCollectionLogger, updateLogger, deleteLogger) => _readMemberBySteamIdSender = new(connection, readBySteamIdLogger);

    public async Task<Member?> ReadBySteamId(ulong steamId) => await _readMemberBySteamIdSender.CallAsync(steamId).ConfigureAwait(false);
}