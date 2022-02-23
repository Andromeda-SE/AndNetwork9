using And9.Lib.Broker;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Database;
using And9.Service.Election.Database.Models;
using And9.Service.Election.Senders;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace And9.Service.Election.Listeners;

public class CancelRegisterListener : BaseRabbitListenerWithResponse<int, bool>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public CancelRegisterListener(IConnection connection, ILogger<BaseRabbitListenerWithResponse<int, bool>> logger, IServiceScopeFactory serviceScopeFactory) 
        : base(connection, CancelRegisterSender.QUEUE_NAME, logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<bool> GetResponseAsync(int request)
    {
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        ElectionDataContext context = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();

        (short id, ElectionStatus status) = await context.GetCurrentElectionStatusAsync().ConfigureAwait(false);
        if (status != ElectionStatus.Registration) return false;
        ElectionVote[] candidateRows =
            await context.ElectionVotes
                .Where(x => x.ElectionId == id && x.MemberId == request && x.Voted == null)
                .ToArrayAsync().ConfigureAwait(false);
        if (candidateRows.Length == 0) return false;
        context.ElectionVotes.RemoveRange(candidateRows);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }
}