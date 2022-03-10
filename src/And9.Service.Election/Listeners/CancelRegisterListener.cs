using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Abstractions.Models;
using And9.Service.Election.Database;
using And9.Service.Election.Senders;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace And9.Service.Election.Listeners;

public class CancelRegisterListener : BaseRabbitListenerWithResponse<int, bool>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CancelRegisterListener(IConnection connection, ILogger<BaseRabbitListenerWithResponse<int, bool>> logger, IServiceScopeFactory serviceScopeFactory)
        : base(connection, CancelRegisterSender.QUEUE_NAME, logger) => _serviceScopeFactory = serviceScopeFactory;

    protected override async Task<bool> GetResponseAsync(int request)
    {
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        ElectionDataContext context = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();
        List<int> electionsNeedUpdate = new(8);
        await foreach ((short id, _, ElectionStatus status) in context.GetCurrentElectionStatusAsync().ConfigureAwait(false))
        {
            if (status != ElectionStatus.Registration) continue;
            ElectionVote[] candidateRows =
                await context.ElectionVotes
                    .Where(x => x.ElectionId == id && x.MemberId == request && x.Voted == null)
                    .ToArrayAsync().ConfigureAwait(false);
            if (candidateRows.Length == 0) continue;
            context.ElectionVotes.RemoveRange(candidateRows);
            electionsNeedUpdate.Add(id);
        }

        if (!electionsNeedUpdate.Any()) return false;
        await context.SaveChangesAsync().ConfigureAwait(false);
        RaiseElectionUpdateSender sender = scope.ServiceProvider.GetRequiredService<RaiseElectionUpdateSender>();
        await Task.WhenAll(electionsNeedUpdate.Select(x => sender.CallAsync(x))).ConfigureAwait(false);
        return true;
    }
}