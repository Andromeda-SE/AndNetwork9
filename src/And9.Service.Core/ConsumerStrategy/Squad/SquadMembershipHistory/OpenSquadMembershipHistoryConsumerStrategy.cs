using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad.SquadMembershipHistory;

namespace And9.Service.Core.ConsumerStrategy.Squad.SquadMembershipHistory;

[QueueName(OpenSquadMembershipHistorySender.QUEUE_NAME)]
public class OpenSquadMembershipHistoryConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<(int memberId, short squadNumber)>
{
    private readonly CoreDataContext _coreDataContext;
    public OpenSquadMembershipHistoryConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask ExecuteAsync((int memberId, short squadNumber) arg)
    {
        (int memberId, short squadNumber) = arg;
        await _coreDataContext.SquadMembershipHistory.AddAsync(new()
        {
            JoinDateTime = DateTime.UtcNow,
            LeaveDateTime = null,
            MemberId = memberId,
            SquadId = squadNumber,
        }).ConfigureAwait(false);
        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}