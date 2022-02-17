using System.Runtime.CompilerServices;
using And9.Gateway.Clan.Senders;
using And9.Lib.Utility;
using And9.Service.Award.Database;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Senders;

namespace And9.Service.Award.Services;

public sealed class RankUpdaterService : TimerService
{
    private readonly IServiceScopeFactory _scopeFactory;
    public RankUpdaterService(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;
    protected override TimeSpan Interval => TimeSpan.FromSeconds(30);

    protected override async Task Process()
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        AwardDataContext awardDataContext = scope.ServiceProvider.GetRequiredService<AwardDataContext>();
        MemberCrudSender memberCrudSender = scope.ServiceProvider.GetRequiredService<MemberCrudSender>();
        RaiseMemberUpdateSender raiseMemberUpdateSender = scope.ServiceProvider.GetRequiredService<RaiseMemberUpdateSender>();


        await foreach ((int memberId, double points) in awardDataContext.GetPointsAsync(CancellationToken.None).ConfigureAwait(false))
        {
            if ((await memberCrudSender.Read(memberId).ConfigureAwait(false))?.Rank is <= Rank.None or >= Rank.Advisor or null) continue;
            await raiseMemberUpdateSender.CallAsync(new()
            {
                MemberId = memberId,
                Points = points,
            }).ConfigureAwait(false);
        }
    }
}