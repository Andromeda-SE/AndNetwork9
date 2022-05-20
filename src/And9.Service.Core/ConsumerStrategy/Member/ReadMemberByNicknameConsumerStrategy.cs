using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Database;
using And9.Service.Core.Senders.Member;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.ConsumerStrategy.Member;

[QueueName(ReadMemberByNicknameSender.QUEUE_NAME)]
public class ReadMemberByNicknameConsumerStrategy : IBrokerConsumerWithResponseStrategy<string, Abstractions.Models.Member?>
{
    private readonly CoreDataContext _coreDataContext;

    public ReadMemberByNicknameConsumerStrategy(CoreDataContext coreDataContext) => _coreDataContext = coreDataContext;


    public async ValueTask<Abstractions.Models.Member?> ExecuteAsync(string? request) => await _coreDataContext.Members.FirstOrDefaultAsync(x => x.Nickname == request).ConfigureAwait(false);
}