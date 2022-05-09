using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy;

[QueueName(ReadMemberByDiscordIdSender.QUEUE_NAME)]
public class ReadMemberByDiscordIdConsumerStrategy : IBrokerConsumerWithResponseStrategy<ulong, Member?>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadMemberByDiscordIdConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;

    public async ValueTask<Member?> ExecuteAsync(ulong request) => await _coreDataContext.Members.FirstOrDefaultAsync(x => x.DiscordId == request).ConfigureAwait(false);
}