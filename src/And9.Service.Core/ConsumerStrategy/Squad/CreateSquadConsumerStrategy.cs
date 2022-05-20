using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Squad;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace And9.Service.Core.ConsumerStrategy.Squad;

[QueueName(CreateSquadSender.QUEUE_NAME)]
public class CreateSquadConsumerStrategy : IBrokerConsumerWithResponseStrategy<short, short>
{
    private readonly CoreDataContext _coreDataContext;
    public CreateSquadConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<short> ExecuteAsync(short arg)
    {
        EntityEntry<Abstractions.Models.Squad> result = (await _coreDataContext.Squads.AddAsync(new()
        {
            Names = new(),
            CreateDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Members = new List<Abstractions.Models.Member>(),
            IsActive = true,
            SquadMembershipHistoryEntries = new List<SquadMembershipHistoryEntry>(),
            SquadRequests = new List<Abstractions.Models.SquadRequest>(),
            LastChanged = DateTime.UtcNow,
            ConcurrencyToken = Guid.NewGuid(),
        }).ConfigureAwait(false))!;
        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        return result.Entity.Number;
    }
}