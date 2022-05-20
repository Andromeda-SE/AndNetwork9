using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.CandidateRequest;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.CandidateRequests;

[QueueName(ReadAllCandidateRequestSender.QUEUE_NAME)]
public class ReadAllCandidateRequestConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<int, CandidateRegisteredRequest>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadAllCandidateRequestConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async IAsyncEnumerable<CandidateRegisteredRequest> ExecuteAsync(int _)
    {
        await foreach (CandidateRegisteredRequest registeredRequest in _coreDataContext.CandidateRequests.AsNoTracking().AsAsyncEnumerable().ConfigureAwait(false)) yield return registeredRequest;
    }
}