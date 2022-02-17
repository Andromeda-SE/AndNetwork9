using System.Collections.ObjectModel;
using And9.Gateway.Clan.Hubs.Model;
using And9.Gateway.Clan.Senders;
using And9.Gateway.Clan.Senders.Models;
using And9.Lib.Broker;
using And9.Lib.Models.Abstractions;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;

namespace And9.Gateway.Clan.Listeners;

public class RaiseMemberUpdateListener : BaseRabbitListenerWithResponse<RaiseMemberUpdateArg, Rank>
{
    public static readonly IReadOnlyDictionary<Rank, int> RankPoints = new ReadOnlyDictionary<Rank, int>(
        new Dictionary<Rank, int>(new[]
        {
            new KeyValuePair<Rank, int>(Rank.Neophyte, 0),
            new KeyValuePair<Rank, int>(Rank.Trainee, 5),
            new KeyValuePair<Rank, int>(Rank.Assistant, 10),
            new KeyValuePair<Rank, int>(Rank.JuniorEmployee, 15),
            new KeyValuePair<Rank, int>(Rank.Employee, 20),
            new KeyValuePair<Rank, int>(Rank.SeniorEmployee, 25),
            new KeyValuePair<Rank, int>(Rank.JuniorSpecialist, 35),
            new KeyValuePair<Rank, int>(Rank.Specialist, 45),
            new KeyValuePair<Rank, int>(Rank.SeniorSpecialist, 55),
            new KeyValuePair<Rank, int>(Rank.JuniorIntercessor, 70),
            new KeyValuePair<Rank, int>(Rank.Intercessor, 85),
            new KeyValuePair<Rank, int>(Rank.SeniorIntercessor, 105),
            new KeyValuePair<Rank, int>(Rank.JuniorSentinel, 120),
            new KeyValuePair<Rank, int>(Rank.Sentinel, 135),
            new KeyValuePair<Rank, int>(Rank.SeniorSentinel, 150),
        }));

    private readonly MemberCrudSender _memberCrudSender;
    private readonly IHubContext<MemberHub> _memberHub;

    public RaiseMemberUpdateListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithResponse<RaiseMemberUpdateArg, Rank>> logger,
        MemberCrudSender memberCrudSender,
        IHubContext<MemberHub> memberHub)
        : base(connection, RaiseMemberUpdateSender.QUEUE_NAME, logger)
    {
        _memberCrudSender = memberCrudSender;
        _memberHub = memberHub;
    }

    protected override async Task<Rank> GetResponseAsync(RaiseMemberUpdateArg request)
    {
        Member? member = await _memberCrudSender.Read(request.MemberId).ConfigureAwait(false);
        if (member is null) throw new ArgumentException("member not found", nameof(request));
        if (member.Rank is < Rank.Neophyte or >= Rank.Advisor) return member.Rank;
        Rank oldRank = member.Rank;
        member.Rank = request.Points >= 0
            ? RankPoints.Where(x => x.Value <= request.Points).MaxBy(x => x.Value).Key
            : RankPoints.MinBy(x => x.Value).Key;
        if (oldRank != member.Rank)
        {
            await _memberCrudSender.Update(member).ConfigureAwait(false);
            await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), member.Id, ModelState.Updated).ConfigureAwait(false);
        }

        return member.Rank;
    }
}