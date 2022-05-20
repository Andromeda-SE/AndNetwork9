using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Member;

namespace And9.Service.Core.ConsumerStrategy.Member;

[QueueName(UpdateMemberSender.QUEUE_NAME)]
public class UpdateMemberConsumerStrategy : IBrokerConsumerWithResponseStrategy<Abstractions.Models.Member, Abstractions.Models.Member>
{
    private readonly CoreDataContext _coreDataContext;

    public UpdateMemberConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<Abstractions.Models.Member> ExecuteAsync(Abstractions.Models.Member? entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        Abstractions.Models.Member? member = await _coreDataContext.Members.FindAsync(entity.Id).ConfigureAwait(false);
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

        if (!entity.Specializations.SequenceEqual(member.Specializations))
        {
            int maxSpecializations = member.Rank.GetSpecializationMaxCount();
            if (entity.Specializations.Count > maxSpecializations) throw new InvalidOperationException("Specializations limit exceeded");
            if (entity.Specializations.Any(x => x.Priority is null)) throw new ArgumentNullException(nameof(MemberSpecialization.Priority));
            if (entity.Specializations.GroupBy(x => x.Priority).Any(x => x.Count() > 1)) throw new InvalidOperationException("Priority is duplicated");
            foreach (MemberSpecialization memberSpecialization in _coreDataContext.MemberSpecializations.Where(x => x.MemberId == member.Id))
            {
                memberSpecialization.Priority = null;
            }

            foreach (MemberSpecialization entitySpecialization in entity.Specializations)
            {
                if (entitySpecialization.Priority >= maxSpecializations) throw new ArgumentException(nameof(entitySpecialization.Priority));
                Specialization? specialization = await _coreDataContext.Specializations.FindAsync(entitySpecialization.SpecializationId).ConfigureAwait(false);
                if (specialization is null) throw new KeyNotFoundException();

                MemberSpecialization? memberSpecialization = await _coreDataContext.MemberSpecializations
                    .FindAsync(new { entity.Id, entitySpecialization.SpecializationId }).ConfigureAwait(false);

                if (memberSpecialization is null)
                {
                    memberSpecialization = new()
                    {
                        MemberId = entity.Id,
                        ApproveDateTime = null,
                        SpecializationId = entitySpecialization.SpecializationId,
                        Priority = entitySpecialization.Priority
                    };
                    await _coreDataContext.MemberSpecializations.AddAsync(memberSpecialization).ConfigureAwait(false);
                }
                if (specialization.MustApproved && memberSpecialization.ApproveDateTime is null) 
                    throw new InvalidOperationException($"«{specialization.Name}» specialization must be approved");
                memberSpecialization.Priority = entitySpecialization.Priority;
            }
        }

        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        return member;
    }
}