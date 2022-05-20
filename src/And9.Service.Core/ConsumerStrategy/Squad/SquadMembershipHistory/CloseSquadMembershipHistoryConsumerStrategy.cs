using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad.SquadMembershipHistory;

namespace And9.Service.Core.ConsumerStrategy.Squad.SquadMembershipHistory;

[QueueName(CloseSquadMembershipHistorySender.QUEUE_NAME)]
public class CloseSquadMembershipHistoryConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<int>
{
    private readonly CoreDataContext _coreDataContext;

    public CloseSquadMembershipHistoryConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask ExecuteAsync(int arg)
    {
        DateTime now = DateTime.UtcNow;
        foreach (SquadMembershipHistoryEntry squadMembershipHistoryEntry in _coreDataContext.SquadMembershipHistory.Where(x => x.MemberId == arg)) squadMembershipHistoryEntry.LeaveDateTime = now;
        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}