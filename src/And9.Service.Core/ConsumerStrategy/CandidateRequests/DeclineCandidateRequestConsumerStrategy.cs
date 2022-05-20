using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.CandidateRequest;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.CandidateRequests;

[QueueName(DeclineCandidateRequestSender.QUEUE_NAME)]
public class DeclineCandidateRequestConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<int>
{
    private readonly CoreDataContext _coreDataContext;
    public DeclineCandidateRequestConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask ExecuteAsync(int requestId)
    {
        CandidateRegisteredRequest? request = await _coreDataContext.CandidateRequests.FirstAsync(x => x.Id == requestId).ConfigureAwait(false);
        if (request is null) throw new ArgumentException("Request not found", nameof(requestId));

        request.Accepted = false;

        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}