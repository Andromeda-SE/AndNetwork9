using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad.SquadRequest;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.Squad.SquadRequest;

[QueueName(DeclineSquadJoinRequestSender.QUEUE_NAME)]
public class DeclineSquadJoinRequestConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<(short number, int memberId, bool byMember)>
{
    private readonly CoreDataContext _coreDataContext;
    public DeclineSquadJoinRequestConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask ExecuteAsync((short number, int memberId, bool byMember) arg)
    {
        (short number, int memberId, bool byMember) = arg;
        Abstractions.Models.SquadRequest? squadRequest = await _coreDataContext.SquadRequests.FirstOrDefaultAsync(x => x.SquadNumber == number && x.MemberId == memberId).ConfigureAwait(false);
        if (squadRequest is null) throw new KeyNotFoundException();
        _coreDataContext.SquadRequests.Update(squadRequest with
        {
            Accepted = false,
            IsCanceledByMember = byMember,
        });
        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}