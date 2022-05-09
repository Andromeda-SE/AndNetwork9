using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Abstractions.Models;
using And9.Service.Election.Database;
using And9.Service.Election.Senders;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Election.ConsumerStrategies;

[QueueName(CancelRegisterSender.QUEUE_NAME)]
public class CancelRegisterConsumerStrategy : IBrokerConsumerWithResponseStrategy<int, bool>
{
    private readonly ElectionDataContext _electionDataContext;
    private readonly RaiseElectionUpdateSender _raiseElectionUpdateSender;

    public CancelRegisterConsumerStrategy(ElectionDataContext electionDataContext, RaiseElectionUpdateSender raiseElectionUpdateSender)
    {
        _electionDataContext = electionDataContext;
        _raiseElectionUpdateSender = raiseElectionUpdateSender;
    }

    public async ValueTask<bool> ExecuteAsync(int request)
    {
        List<int> electionsNeedUpdate = new(8);
        await foreach ((short id, _, ElectionStatus status) in _electionDataContext.GetCurrentElectionStatusAsync().ConfigureAwait(false))
        {
            if (status != ElectionStatus.Registration) continue;
            ElectionVote[] candidateRows =
                await _electionDataContext.ElectionVotes
                    .Where(x => x.ElectionId == id && x.MemberId == request && x.Voted == null)
                    .ToArrayAsync().ConfigureAwait(false);
            if (candidateRows.Length == 0) continue;
            _electionDataContext.ElectionVotes.RemoveRange(candidateRows);
            electionsNeedUpdate.Add(id);
        }

        if (!electionsNeedUpdate.Any()) return false;
        await _electionDataContext.SaveChangesAsync().ConfigureAwait(false);

        await Task.WhenAll(electionsNeedUpdate.Select(x => _raiseElectionUpdateSender.CallAsync(x).AsTask())).ConfigureAwait(false);
        return true;
    }
}