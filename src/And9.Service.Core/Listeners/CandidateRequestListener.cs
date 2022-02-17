using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Lib.Broker.Crud;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using RabbitMQ.Client;

namespace And9.Service.Core.Listeners;

public class CandidateRequestListener : RabbitCrudListener<CandidateRegisteredRequest>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CandidateRequestListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithResponse<CandidateRegisteredRequest, int>> createLogger,
        ILogger<BaseRabbitListenerWithResponse<int, CandidateRegisteredRequest>> readLogger,
        ILogger<BaseRabbitListenerWithStreamResponse<object, CandidateRegisteredRequest>> readAllLogger,
        ILogger<BaseRabbitListenerWithResponse<CandidateRegisteredRequest, CandidateRegisteredRequest>> updateLogger,
        ILogger<BaseRabbitListenerWithoutResponse<int>> deleteLogger,
        IServiceScopeFactory scopeFactory)
        : base(connection, CandidateRequestCrudSender.QUEUE_NAME, CandidateRequestCrudSender.CRUD_FLAGS, createLogger, readLogger, readAllLogger, updateLogger, deleteLogger) => _scopeFactory = scopeFactory;

    public override Task<int> Create(CandidateRegisteredRequest entity) => throw new NotSupportedException();

    public override Task<CandidateRegisteredRequest> Update(CandidateRegisteredRequest entity) => throw new NotSupportedException();

    public override async Task<CandidateRegisteredRequest?> Read(int id)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();
        return await coreDataContext.CandidateRequests.FindAsync(id).ConfigureAwait(false);
    }

    public override async IAsyncEnumerable<CandidateRegisteredRequest> ReadAll(object arg, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        await foreach (CandidateRegisteredRequest registeredRequest in coreDataContext.CandidateRequests.AsAsyncEnumerable().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return registeredRequest;
        }
    }

    public override Task Delete(int id) => throw new NotSupportedException();
}