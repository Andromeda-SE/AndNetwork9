using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy;

[QueueName(AcceptCandidateRequestSender.QUEUE_NAME)]
public class AcceptCandidateRequestConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<int>
{
    private readonly CoreDataContext _coreDataContext;
    private readonly RaiseMemberUpdateSender _raiseMemberUpdateSender;

    public AcceptCandidateRequestConsumerStrategy(CoreDataContext coreDataContext, RaiseMemberUpdateSender raiseMemberUpdateSender)
    {
        _coreDataContext = coreDataContext;
        _raiseMemberUpdateSender = raiseMemberUpdateSender;
    }

    public async ValueTask ExecuteAsync(int requestId)
    {
        CandidateRegisteredRequest? request = await _coreDataContext.CandidateRequests.Include(x => x.Member).FirstAsync(x => x.Id == requestId).ConfigureAwait(false);
        if (request is null) throw new ArgumentException("Request not found", nameof(requestId));
        if (request.AuxiliarySquad is not null) throw new ArgumentException("Auxiliary request", nameof(requestId));

        request.Accepted = true;
        request.Member.Rank = Rank.Neophyte;
        request.Member.JoinDate = DateOnly.FromDateTime(DateTime.UtcNow);

        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        await _raiseMemberUpdateSender.CallAsync(new()
        {
            MemberId = request.MemberId,
            Points = 0D,
        }).ConfigureAwait(false);
    }
}