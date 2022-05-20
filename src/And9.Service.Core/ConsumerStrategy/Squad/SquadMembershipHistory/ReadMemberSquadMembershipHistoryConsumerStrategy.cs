using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad.SquadMembershipHistory;

namespace And9.Service.Core.ConsumerStrategy.Squad.SquadMembershipHistory;

[QueueName(ReadMemberSquadMembershipHistorySender.QUEUE_NAME)]
public class ReadMemberSquadMembershipHistoryConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<int, ISquadMembershipHistoryEntry>
{
    private readonly CoreDataContext _coreDataContext;
    public ReadMemberSquadMembershipHistoryConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async IAsyncEnumerable<ISquadMembershipHistoryEntry> ExecuteAsync(int arg)
    {
        await foreach (SquadMembershipHistoryEntry squadMembershipHistoryEntry in _coreDataContext.SquadMembershipHistory
                           .Where(x => x.MemberId == arg)
                           .OrderBy(x => x.JoinDateTime)
                           .ToAsyncEnumerable().ConfigureAwait(false))
            yield return squadMembershipHistoryEntry;
    }
}