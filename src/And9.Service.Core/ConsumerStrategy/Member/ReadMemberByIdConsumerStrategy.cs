using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Member;

namespace And9.Service.Core.ConsumerStrategy.Member;

[QueueName(ReadMemberByIdSender.QUEUE_NAME)]
public class ReadMemberByIdConsumerStrategy : IBrokerConsumerWithResponseStrategy<int, Abstractions.Models.Member?>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadMemberByIdConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<Abstractions.Models.Member?> ExecuteAsync(int id) => await _coreDataContext.Members.FindAsync(id).ConfigureAwait(false);
}