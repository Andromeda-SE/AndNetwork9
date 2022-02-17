using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace And9.Service.Core.Listeners;

public class DeclineCandidateListener : BaseRabbitListenerWithoutResponse<int>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DeclineCandidateListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithoutResponse<int>> logger, IServiceScopeFactory scopeFactory)
        : base(connection, DeclineCandidateRequestSender.QUEUE_NAME, logger) => _scopeFactory = scopeFactory;

    public override async Task Run(int requestId)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        CandidateRegisteredRequest? request = await coreDataContext.CandidateRequests.FirstAsync(x => x.Id == requestId).ConfigureAwait(false);
        if (request is null) throw new ArgumentException("Request not found", nameof(requestId));

        request.Accepted = false;

        await coreDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}