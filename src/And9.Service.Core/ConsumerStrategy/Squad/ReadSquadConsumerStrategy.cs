using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad;

namespace And9.Service.Core.ConsumerStrategy.Squad;

[QueueName(ReadSquadSender.QUEUE_NAME)]
public class ReadSquadConsumerStrategy : IBrokerConsumerWithResponseStrategy<int, Abstractions.Models.Squad?>
{
    private readonly CoreDataContext _coreDataContext;
    public ReadSquadConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<Abstractions.Models.Squad?> ExecuteAsync(int arg) => await _coreDataContext.Squads.FindAsync(arg).ConfigureAwait(false);
}