using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Member;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.Member;

[QueueName(ReadMemberBySteamIdSender.QUEUE_NAME)]
public class ReadMemberBySteamIdConsumerStrategy : IBrokerConsumerWithResponseStrategy<ulong, Abstractions.Models.Member?>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadMemberBySteamIdConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<Abstractions.Models.Member?> ExecuteAsync(ulong request) => await _coreDataContext.Members.FirstOrDefaultAsync(x => x.SteamId == request).ConfigureAwait(false);
}