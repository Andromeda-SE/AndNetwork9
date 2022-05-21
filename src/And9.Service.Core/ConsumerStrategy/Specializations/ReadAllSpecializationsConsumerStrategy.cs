using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Specializations;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.Specializations;

[QueueName(ReadAllSpecializationsSender.QUEUE_NAME)]
public class ReadAllSpecializationsConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<int, Specialization>
{
    private readonly CoreDataContext _coreDataContext;
    public ReadAllSpecializationsConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;
    public IAsyncEnumerable<Specialization> ExecuteAsync(int arg) => _coreDataContext.Specializations.AsNoTracking().AsAsyncEnumerable();
}