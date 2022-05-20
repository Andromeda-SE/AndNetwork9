using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad.SquadRequest;

namespace And9.Service.Core.ConsumerStrategy.Squad.SquadRequest;

[QueueName(ReadMemberSquadRequestSender.QUEUE_NAME)]
public class ReadMemberSquadRequestConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<int, Abstractions.Models.SquadRequest>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadMemberSquadRequestConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async IAsyncEnumerable<Abstractions.Models.SquadRequest> ExecuteAsync(int arg)
    {
        await foreach (Abstractions.Models.SquadRequest squadRequest in _coreDataContext.SquadRequests.Where(x => x.MemberId == arg).ToAsyncEnumerable().ConfigureAwait(false)) yield return squadRequest;
    }
}