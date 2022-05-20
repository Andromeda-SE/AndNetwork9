using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad.SquadRequest;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.Squad.SquadRequest;

[QueueName(SendSquadJoinRequestSender.QUEUE_NAME)]
public class SendSquadJoinRequestConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<(int memberId, short squadNumber)>
{
    private readonly CoreDataContext _coreDataContext;

    public SendSquadJoinRequestConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask ExecuteAsync((int memberId, short squadNumber) arg)
    {
        Abstractions.Models.Member? member = await _coreDataContext.Members.FindAsync(arg.memberId).ConfigureAwait(false);
        if (member is null) throw new ArgumentNullException(nameof(arg.memberId));
        if (member.SquadNumber is not null) throw new InvalidOperationException();
        if (await _coreDataContext.SquadRequests
                .AnyAsync(x => x.SquadNumber == arg.squadNumber
                               && x.MemberId == arg.memberId
                               && x.Accepted == null).ConfigureAwait(false))
            throw new ArgumentException("request is not closed");
        await _coreDataContext.SquadRequests.AddAsync(new()
        {
            SquadNumber = arg.squadNumber,
            MemberId = arg.memberId,
            Accepted = null,
            IsCanceledByMember = false,
        }).ConfigureAwait(false);
    }
}