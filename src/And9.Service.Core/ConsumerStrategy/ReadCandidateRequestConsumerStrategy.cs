using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;

namespace And9.Service.Core.ConsumerStrategy;

[QueueName(ReadCandidateRequestSender.QUEUE_NAME)]
public class ReadCandidateRequestConsumerStrategy : IBrokerConsumerWithResponseStrategy<int, CandidateRegisteredRequest?>
{
    private readonly CoreDataContext _coreDataContext;
    public ReadCandidateRequestConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<CandidateRegisteredRequest?> ExecuteAsync(int id) => await _coreDataContext.CandidateRequests.FindAsync(id).ConfigureAwait(false);
}