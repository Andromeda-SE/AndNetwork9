using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace And9.Service.Core.ConsumerStrategy;

[QueueName(CreateMemberSender.QUEUE_NAME)]
public class CreateMemberConsumerStrategy : IBrokerConsumerWithResponseStrategy<Member, int>
{
    private readonly CoreDataContext _coreDataContext;
    public CreateMemberConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<int> ExecuteAsync(Member? entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        EntityEntry<Member> result = await _coreDataContext.Members.AddAsync(new()
        {
            Direction = entity.Direction,
            DiscordId = entity.DiscordId,
            IsSquadCommander = entity.IsSquadCommander,
            JoinDate = entity.JoinDate,
            LastDirectionChange = DateOnly.FromDateTime(DateTime.UtcNow),
            MicrosoftId = entity.MicrosoftId,
            Nickname = entity.Nickname,
            Rank = entity.Rank,
            RealName = entity.RealName,
            SquadNumber = entity.SquadNumber,
            SquadPartNumber = entity.SquadPartNumber,
            SteamId = entity.SteamId,
            TelegramId = entity.TelegramId,
            TimeZone = entity.TimeZone,
            VkId = entity.VkId,
        }).ConfigureAwait(false);
        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        return result.Entity.Id;
    }
}