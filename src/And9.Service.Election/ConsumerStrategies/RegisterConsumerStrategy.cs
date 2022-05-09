using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Database;
using And9.Service.Election.Senders;

namespace And9.Service.Election.ConsumerStrategies;

[QueueName(RegisterSender.QUEUE_NAME)]
public class RegisterConsumerStrategy : IBrokerConsumerWithResponseStrategy<int, bool>
{
    private readonly ElectionDataContext _electionDataContext;
    private readonly RaiseElectionUpdateSender _raiseElectionUpdateSender;
    private readonly ReadMemberByIdSender _readMemberByIdSender;

    public RegisterConsumerStrategy(ElectionDataContext electionDataContext, RaiseElectionUpdateSender raiseElectionUpdateSender, ReadMemberByIdSender readMemberByIdSender)
    {
        _electionDataContext = electionDataContext;
        _raiseElectionUpdateSender = raiseElectionUpdateSender;
        _readMemberByIdSender = readMemberByIdSender;
    }

    public async ValueTask<bool> ExecuteAsync(int request)
    {
        Member? member = await _readMemberByIdSender.CallAsync(request).ConfigureAwait(false);
        if (member?.Rank is null or <= Rank.None or Rank.FirstAdvisor) return false;
        if (member.Direction <= Direction.None) return false;

        (short id, _, ElectionStatus status) = await _electionDataContext.GetCurrentElectionStatusAsync(member.Direction).ConfigureAwait(false);
        if (status is not ElectionStatus.Registration) return false;
        if (_electionDataContext.ElectionVotes.Any(x => x.MemberId == request)) return false;

        _electionDataContext.ElectionVotes.Add(new()
        {
            Direction = member.Direction,
            ElectionId = id,
            MemberId = member.Id,
            Voted = null,
            Votes = 0,
        });
        await _electionDataContext.SaveChangesAsync().ConfigureAwait(false);
        await _raiseElectionUpdateSender.CallAsync(id).ConfigureAwait(false);
        return true;
    }
}