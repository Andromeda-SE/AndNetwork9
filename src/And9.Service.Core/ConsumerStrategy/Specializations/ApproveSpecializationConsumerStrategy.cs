using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Specializations;

namespace And9.Service.Core.ConsumerStrategy.Specializations;

[QueueName(ApproveSpecializationSender.QUEUE_NAME)]
public class ApproveSpecializationConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<(int memberId, int specializationId)>
{
    private readonly CoreDataContext _coreDataContext;
    public ApproveSpecializationConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask ExecuteAsync((int memberId, int specializationId) arg)
    {
        (int memberId, int specializationId) = arg;
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