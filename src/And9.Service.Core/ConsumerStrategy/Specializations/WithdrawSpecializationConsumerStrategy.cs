using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Specializations;

namespace And9.Service.Core.ConsumerStrategy.Specializations;

[QueueName(WithdrawSpecializationSender.QUEUE_NAME)]
public class WithdrawSpecializationConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<(int memberId, int specializationId)>
{
    private readonly CoreDataContext _coreDataContext;
    public WithdrawSpecializationConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask ExecuteAsync((int memberId, int specializationId) arg)
    {
        (int memberId, int specializationId) = arg;
        MemberSpecialization? memberSpecialization = 
            await _coreDataContext.MemberSpecializations.FindAsync(new { memberId, specializationId }).ConfigureAwait(false);
        if (memberSpecialization is null) return;
        _coreDataContext.MemberSpecializations.Remove(memberSpecialization);
        await _coreDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}