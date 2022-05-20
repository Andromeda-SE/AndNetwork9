using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad.SquadMembershipHistory;

namespace And9.Service.Core.ConsumerStrategy.Squad.SquadMembershipHistory;

[QueueName(ReadSquadMembershipHistorySender.QUEUE_NAME)]
public class ReadSquadMembershipHistoryConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<short, ISquadMembershipHistoryEntry>
{
    private readonly CoreDataContext _coreDataContext;
    public ReadSquadMembershipHistoryConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async IAsyncEnumerable<ISquadMembershipHistoryEntry> ExecuteAsync(short arg)
    {
        await foreach (SquadMembershipHistoryEntry squadMembershipHistoryEntry in _coreDataContext.SquadMembershipHistory
                           .Where(x => x.SquadId == arg)
                           .OrderBy(x => x.JoinDateTime)
                           .ToAsyncEnumerable().ConfigureAwait(false))
            yield return squadMembershipHistoryEntry;
    }
}