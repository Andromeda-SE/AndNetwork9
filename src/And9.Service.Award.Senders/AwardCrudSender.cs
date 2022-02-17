using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Lib.Broker.Crud;
using And9.Lib.Broker.Crud.Sender;
using And9.Service.Award.Senders.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Award.Senders;

public class AwardCrudSender : RabbitCrudSender<Abstractions.Models.Award>, IAwardModelServiceMethods
{
    public const string QUEUE_NAME = "And9.Service.Award.Award";
    public const CrudFlags CRUD_FLAGS = CrudFlags.Create | CrudFlags.Read;

    public AwardCrudSender(IConnection connection,
        ILogger<BaseRabbitSenderWithResponse<Abstractions.Models.Award, int>> createLogger,
        ILogger<BaseRabbitSenderWithResponse<int, Abstractions.Models.Award>> readLogger,
        ILogger<BaseRabbitSenderWithStreamResponse<object, Abstractions.Models.Award>> readCollectionLogger,
        ILogger<BaseRabbitSenderWithResponse<Abstractions.Models.Award, Abstractions.Models.Award>> updateLogger,
        ILogger<BaseRabbitSenderWithoutResponse<int>> deleteLogger)
        : base(connection, QUEUE_NAME, CRUD_FLAGS, createLogger, readLogger, readCollectionLogger, updateLogger, deleteLogger) => ReadMemberSender = new(connection, QUEUE_NAME, nameof(ReadByMemberId), readCollectionLogger);

    private ReadCollectionSender<Abstractions.Models.Award> ReadMemberSender { get; }


    public async IAsyncEnumerable<Abstractions.Models.Award> ReadByMemberId(int memberId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (Abstractions.Models.Award award in (ReadMemberSender.CallAsync(memberId) ?? throw new()).WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return award;
        }
    }
}