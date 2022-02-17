using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace And9.Service.Core.Listeners;

public class AcceptCandidateListener : BaseRabbitListenerWithoutResponse<int>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AcceptCandidateListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithoutResponse<int>> logger,
        IServiceScopeFactory scopeFactory)
        : base(connection, AcceptCandidateRequestSender.QUEUE_NAME, logger) => _scopeFactory = scopeFactory;

    public override async Task Run(int requestId)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        CandidateRegisteredRequest? request = await coreDataContext.CandidateRequests.Include(x => x.Member).FirstAsync(x => x.Id == requestId).ConfigureAwait(false);
        if (request is null) throw new ArgumentException("Request not found", nameof(requestId));
        if (request.AuxiliarySquad is not null) throw new ArgumentException("Auxiliary request", nameof(requestId));

        request.Accepted = true;
        request.Member.Rank = Rank.Neophyte;
        request.Member.Direction = Direction.Training;
        request.Member.JoinDate = request.Member.LastDirectionChange = DateOnly.FromDateTime(DateTime.UtcNow);

        await coreDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}