using And9.Gateway.Clan.Senders;
using And9.Service.Award.Database;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Senders;
using Quartz;

namespace And9.Service.Award.Jobs;

[DisallowConcurrentExecution]
public sealed class RankUpdateJob : IJob
{
    private readonly AwardDataContext _awardDataContext;
    private readonly RaiseMemberUpdateSender _raiseMemberUpdateSender;
    private readonly ReadMemberByIdSender _readMemberByIdSender;

    public RankUpdateJob(RaiseMemberUpdateSender raiseMemberUpdateSender, AwardDataContext awardDataContext, ReadMemberByIdSender readMemberByIdSender)
    {
        _raiseMemberUpdateSender = raiseMemberUpdateSender;
        _awardDataContext = awardDataContext;
        _readMemberByIdSender = readMemberByIdSender;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await foreach ((int memberId, double points) in _awardDataContext.GetPointsAsync(context.CancellationToken).ConfigureAwait(false))
        {
            if ((await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false))?.Rank is <= Rank.None or >= Rank.Advisor or null) continue;
            await _raiseMemberUpdateSender.CallAsync(new()
            {
                MemberId = memberId,
                Points = points,
            }).ConfigureAwait(false);
        }
    }
}