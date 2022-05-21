using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Specializations;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.Specializations;

[QueueName(ApproveSpecializationSender.QUEUE_NAME)]
public class ApproveSpecializationConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<(int memberId, int specializationId, int callerId)>
{
    private readonly CoreDataContext _coreDataContext;
    public ApproveSpecializationConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask ExecuteAsync((int memberId, int specializationId, int callerId) arg)
    {
        (int memberId, int specializationId, int callerId) = arg;
        Abstractions.Models.Member? caller = await _coreDataContext.Members.FindAsync(callerId).ConfigureAwait(false);
        if (caller is null) throw new ArgumentNullException(nameof(arg.callerId));
        if (caller.Rank != Rank.Advisor) throw new InvalidOperationException();
        int callerSpecializationId = _coreDataContext.MemberSpecializations.Where(x => x.MemberId == callerId && x.Priority != null).MinBy(x => x.Priority)!.SpecializationId;
        Specialization? callerSpecialization = await _coreDataContext.Specializations.FindAsync(callerSpecializationId).ConfigureAwait(false);
        Specialization? memberSpecialization = await _coreDataContext.Specializations.FindAsync(specializationId).ConfigureAwait(false);
        if (callerSpecialization?.Direction != memberSpecialization?.Direction) throw new InvalidOperationException();
        await _coreDataContext.MemberSpecializations.AddAsync(new()
        {
            Priority = null,
            MemberId = memberId,
            SpecializationId = specializationId,
            ApproveDateTime = DateTime.UtcNow,
        }).ConfigureAwait(false);
        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}