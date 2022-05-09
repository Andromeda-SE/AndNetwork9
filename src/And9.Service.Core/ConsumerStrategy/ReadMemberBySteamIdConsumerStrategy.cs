using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy;

[QueueName(ReadMemberBySteamIdSender.QUEUE_NAME)]
public class ReadMemberBySteamIdConsumerStrategy : IBrokerConsumerWithResponseStrategy<ulong, Member?>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadMemberBySteamIdConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<Member?> ExecuteAsync(ulong request) => await _coreDataContext.Members.FirstOrDefaultAsync(x => x.SteamId == request).ConfigureAwait(false);
}