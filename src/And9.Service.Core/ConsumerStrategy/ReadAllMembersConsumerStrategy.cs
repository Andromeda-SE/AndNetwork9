using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy;

[QueueName(ReadAllMembersSender.QUEUE_NAME)]
public class ReadAllMembersConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<int, Member>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadAllMembersConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async IAsyncEnumerable<Member> ExecuteAsync(int _)
    {
        await foreach (Member member in _coreDataContext.Members.Where(x => x.Rank >= Rank.Auxiliary).AsNoTracking().ToAsyncEnumerable().ConfigureAwait(false)) yield return member;
    }
}