using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad.SquadRequest;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.Squad.SquadRequest;

[QueueName(AcceptSquadJoinRequestSender.QUEUE_NAME)]
public class AcceptSquadJoinRequestConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<(short number, short squadPart, int memberId)>
{
    private readonly CoreDataContext _coreDataContext;
    public AcceptSquadJoinRequestConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask ExecuteAsync((short number, short squadPart, int memberId) arg)
    {
        (short number, short squadPart, int memberId) = arg;
        Abstractions.Models.Member? member = await _coreDataContext.Members.FindAsync(memberId).ConfigureAwait(false);
        if (member is null) throw new KeyNotFoundException();
        if (member.SquadNumber is not null) throw new InvalidOperationException();
        if (member.Rank <= Rank.Enemy) throw new InvalidOperationException();
        Abstractions.Models.SquadRequest? squadRequest = await _coreDataContext.SquadRequests.FirstOrDefaultAsync(x => x.SquadNumber == number && x.MemberId == memberId).ConfigureAwait(false);
        if (squadRequest is null) throw new KeyNotFoundException();
        _coreDataContext.SquadRequests.Update(squadRequest with
        {
            Accepted = true,
        });
        _coreDataContext.Members.Update(member with
        {
            SquadNumber = number,
            SquadPartNumber = squadPart,
            Rank = member.Rank <= Rank.None ? Rank.Auxiliary : member.Rank,
        });
        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}