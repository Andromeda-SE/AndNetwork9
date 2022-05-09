using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;

namespace And9.Service.Core.ConsumerStrategy;

[QueueName(ReadMemberByIdSender.QUEUE_NAME)]
public class ReadMemberByIdConsumerStrategy : IBrokerConsumerWithResponseStrategy<int, Member?>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadMemberByIdConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<Member?> ExecuteAsync(int id) => await _coreDataContext.Members.FindAsync(id).ConfigureAwait(false);
}