using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;

namespace And9.Service.Core.ConsumerStrategy;

[QueueName(UpdateMemberSender.QUEUE_NAME)]
public class UpdateMemberConsumerStrategy : IBrokerConsumerWithResponseStrategy<Member, Member>
{
    private readonly CoreDataContext _coreDataContext;

    public UpdateMemberConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<Member> ExecuteAsync(Member? entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        Member? member = await _coreDataContext.Members.FindAsync(entity.Id).ConfigureAwait(false);
        if (member is null) throw new ArgumentException("Member not found", nameof(entity));

        /*if (member.Direction != entity.Direction)
        {
            member.Direction = entity.Direction;
            member.LastDirectionChange = DateOnly.FromDateTime(DateTime.UtcNow);
        }*/

        if (member.DiscordId != entity.DiscordId) member.DiscordId = entity.DiscordId;
        if (member.IsSquadCommander != entity.IsSquadCommander) member.IsSquadCommander = entity.IsSquadCommander;
        if (member.JoinDate != entity.JoinDate) member.JoinDate = entity.JoinDate;
        if (member.MicrosoftId != entity.MicrosoftId) member.MicrosoftId = entity.MicrosoftId;
        if (member.Nickname != entity.Nickname) member.Nickname = entity.Nickname;
        if (member.Rank != entity.Rank) member.Rank = entity.Rank;
        if (member.RealName != entity.RealName) member.RealName = entity.RealName;
        if (member.SquadNumber != entity.SquadNumber) member.SquadNumber = entity.SquadNumber;
        if (member.SquadPartNumber != entity.SquadPartNumber) member.SquadPartNumber = entity.SquadPartNumber;
        if (member.SteamId != entity.SteamId) member.SteamId = entity.SteamId;
        if (member.TelegramId != entity.TelegramId) member.TelegramId = entity.TelegramId;
        if (member.TimeZone?.Id != entity.TimeZone?.Id) member.TimeZone = entity.TimeZone;
        if (member.VkId != entity.VkId) member.VkId = entity.VkId;

        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        return member;
    }
}