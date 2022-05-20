using System.Collections.ObjectModel;
using And9.Gateway.Clan.Hubs.Model;
using And9.Gateway.Clan.Senders;
using And9.Gateway.Clan.Senders.Models;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Lib.Models.Abstractions;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders.Member;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.ConsumerStrategies;

[QueueName(RaiseMemberUpdateSender.QUEUE_NAME)]
public class RaiseMemberUpdateConsumerStrategy : IBrokerConsumerWithResponseStrategy<RaiseMemberUpdateArg, Rank>
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

    private readonly IHubContext<MemberHub> _memberHub;

    private readonly ReadMemberByIdSender _readMemberByIdSender;
    private readonly UpdateMemberSender _updateMemberSender;

    public RaiseMemberUpdateConsumerStrategy(
        IHubContext<MemberHub> memberHub,
        ReadMemberByIdSender readMemberByIdSender,
        UpdateMemberSender updateMemberSender)
    {
        _memberHub = memberHub;
        _readMemberByIdSender = readMemberByIdSender;
        _updateMemberSender = updateMemberSender;
    }

    public async ValueTask<Rank> ExecuteAsync(RaiseMemberUpdateArg request)
    {
        Member? member = await _readMemberByIdSender.CallAsync(request.MemberId).ConfigureAwait(false);
        if (member is null) throw new ArgumentException("member not found", nameof(request));
        if (member.Rank is < Rank.Neophyte or >= Rank.Advisor) return member.Rank;
        Rank oldRank = member.Rank;
        member.Rank = request.Points >= 0
            ? RankPoints.Where(x => x.Value <= request.Points).MaxBy(x => x.Value).Key
            : RankPoints.MinBy(x => x.Value).Key;
        if (oldRank != member.Rank)
        {
            await _updateMemberSender.CallAsync(member).ConfigureAwait(false);
            await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), member.Id, ModelState.Updated).ConfigureAwait(false);
        }

        return member.Rank;
    }
}