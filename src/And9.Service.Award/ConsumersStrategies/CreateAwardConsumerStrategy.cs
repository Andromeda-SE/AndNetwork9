using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Award.Database;
using And9.Service.Award.Senders;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace And9.Service.Award.ConsumersStrategies;

[QueueName(CreateAwardSender.QUEUE_NAME)]
public class CreateAwardConsumerStrategy : IBrokerConsumerWithResponseStrategy<Abstractions.Models.Award, int>
{
    private readonly AwardDataContext _awardDataContext;
    private readonly RaiseMemberUpdateSender _raiseMemberUpdateSender;

    public CreateAwardConsumerStrategy(AwardDataContext awardDataContext, RaiseMemberUpdateSender raiseMemberUpdateSender)
    {
        _awardDataContext = awardDataContext;
        _raiseMemberUpdateSender = raiseMemberUpdateSender;
    }

    public async ValueTask<int> ExecuteAsync(Abstractions.Models.Award? entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        /*Abstractions.Models.Award award = entity with
        {
            Type = entity.Type,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            AutomationTag = null,
            LastChanged = DateTime.UtcNow,
            ConcurrencyToken = Guid.NewGuid(),
        };*/
        Abstractions.Models.Award award = new()
        {
            Type = entity.Type,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            LastChanged = DateTime.UtcNow,
            ConcurrencyToken = Guid.NewGuid(),
            Description = entity.Description,
            GaveById = entity.GaveById,
            MemberId = entity.MemberId,
            AutomationTag = entity.AutomationTag,
        };
        EntityEntry<Abstractions.Models.Award> result =
            await _awardDataContext.Awards.AddAsync(award).ConfigureAwait(false);
        await _awardDataContext.SaveChangesAsync().ConfigureAwait(false);

        await _raiseMemberUpdateSender.CallAsync(new()
        {
            MemberId = award.MemberId,
            Points = await _awardDataContext.GetPointsAsync(result.Entity.MemberId).ConfigureAwait(false),
        }).ConfigureAwait(false);
        return result.Entity.Id;
    }
}