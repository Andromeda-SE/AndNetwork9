using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace And9.Service.Core.ConsumerStrategy.Squad;

[QueueName(UpdateSquadSender.QUEUE_NAME)]
public class UpdateSquadConsumerStrategy : IBrokerConsumerWithResponseStrategy<Abstractions.Models.Squad, Abstractions.Models.Squad>
{
    private readonly CoreDataContext _coreDataContext;
    public UpdateSquadConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<Abstractions.Models.Squad> ExecuteAsync(Abstractions.Models.Squad? arg)
    {
        if (arg is null) throw new ArgumentNullException(nameof(arg));
        Abstractions.Models.Squad? oldSquad = await _coreDataContext.Squads.FindAsync(arg.Number).ConfigureAwait(false);
        if (oldSquad is null) throw new KeyNotFoundException();
        EntityEntry<Abstractions.Models.Squad> updated = _coreDataContext.Squads.Update(oldSquad with
        {
            IsActive = arg.IsActive,
            Names = arg.Names,
        })!;
        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        return updated.Entity;
    }
}