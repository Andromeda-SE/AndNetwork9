using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.Squad;

[QueueName(ReadAllSquadSender.QUEUE_NAME)]
public class ReadAllSquadConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<int, ISquad>
{
    private readonly CoreDataContext _coreDataContext;
    public ReadAllSquadConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async IAsyncEnumerable<ISquad> ExecuteAsync(int arg)
    {
#pragma warning disable CS8634
        await foreach (Abstractions.Models.Squad? squad in _coreDataContext.Squads.AsNoTracking().ToAsyncEnumerable().ConfigureAwait(false))
#pragma warning restore CS8634
            yield return squad!;
    }
}