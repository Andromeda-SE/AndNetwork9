using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad.SquadRequest;

namespace And9.Service.Core.ConsumerStrategy.Squad.SquadRequest;

[QueueName(ReadSquadRequestSender.QUEUE_NAME)]
public class ReadSquadRequestConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<short, Abstractions.Models.SquadRequest>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadSquadRequestConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async IAsyncEnumerable<Abstractions.Models.SquadRequest> ExecuteAsync(short arg)
    {
        await foreach (Abstractions.Models.SquadRequest squadRequest in _coreDataContext.SquadRequests.Where(x => x.SquadNumber == arg).ToAsyncEnumerable().ConfigureAwait(false)) yield return squadRequest;
    }
}