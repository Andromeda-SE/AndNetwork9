using And9.Gateway.Clan.Auth.Attributes;
using And9.Gateway.Clan.Hubs.Model;
using And9.Gateway.Clan.Properties;
using And9.Integration.Discord.Senders;
using And9.Lib.Models.Abstractions;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.API.Interfaces;
using And9.Service.Core.Senders.CandidateRequest;
using And9.Service.Core.Senders.Member;
using And9.Service.Core.Senders.Specializations;
using And9.Service.Core.Senders.Squad;
using And9.Service.Core.Senders.Squad.SquadMembershipHistory;
using And9.Service.Core.Senders.Squad.SquadRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.Hubs;

public class CoreHub : Hub<ICoreClientMethods>, ICoreServerMethods
{
    private readonly AcceptCandidateRequestSender _acceptCandidateRequestSender;

    private readonly AcceptSquadJoinRequestSender _acceptSquadJoinRequestSender;

    private readonly CloseSquadMembershipHistorySender _closeSquadMembershipHistorySender;

    private readonly CreateSquadSender _createSquadSender;
    private readonly DeclineCandidateRequestSender _declineCandidateRequestSender;
    private readonly DeclineSquadJoinRequestSender _declineSquadJoinRequestSender;

    private readonly IHubContext<MemberHub> _memberHub;
    private readonly OpenSquadMembershipHistorySender _openSquadMembershipHistorySender;

    private readonly ReadAllMembersSender _readAllMembersSender;
    private readonly ReadAllSquadSender _readAllSquadSender;
    private readonly ReadCandidateRequestSender _readCandidateRequestSender;
    private readonly ReadMemberByIdSender _readMemberByIdSender;
    private readonly ReadMemberSquadMembershipHistorySender _readMemberSquadMembershipHistorySender;
    private readonly ReadMemberSquadRequestSender _readMemberSquadRequestSender;
    private readonly ReadSquadMembershipHistorySender _readSquadMembershipHistorySender;
    private readonly ReadSquadRequestSender _readSquadRequestSender;
    private readonly ReadSquadSender _readSquadSender;
    private readonly RegisterCandidateRequestSender _registerCandidateRequestSender;


    private readonly SendCandidateRequestSender _sendCandidateRequestSender;

    private readonly SendDirectMessageSender _sendDirectMessageSender;
    private readonly SendLogMessageSender _sendLogMessageSender;
    private readonly SendSquadJoinRequestSender _sendSquadJoinRequestSender;
    private readonly IHubContext<SquadHub> _squadHub;
    private readonly SyncChannelsSender _syncChannelsSender;
    private readonly SyncUserSender _syncUserSender;
    private readonly UpdateMemberSender _updateMemberSender;
    private readonly UpdateSquadSender _updateSquadSender;

    private readonly ReadAllSpecializationsSender _readAllSpecializationsSender;
    private readonly ApproveSpecializationSender _approveSpecializationSender;
    private readonly WithdrawSpecializationSender _withdrawSpecializationSender;

    public CoreHub(
        AcceptCandidateRequestSender acceptCandidateRequestSender,
        DeclineCandidateRequestSender declineCandidateRequestSender,
        RegisterCandidateRequestSender registerCandidateRequestSender,
        ReadCandidateRequestSender readCandidateRequestSender,
        SendCandidateRequestSender sendCandidateRequestSender,
        SendDirectMessageSender sendDirectMessageSender,
        SendLogMessageSender sendLogMessageSender,
        ReadMemberByIdSender readMemberByIdSender,
        UpdateMemberSender updateMemberSender,
        IHubContext<MemberHub> memberHub,
        ReadSquadSender readSquadSender,
        OpenSquadMembershipHistorySender openSquadMembershipHistorySender,
        CloseSquadMembershipHistorySender closeSquadMembershipHistorySender,
        ReadSquadMembershipHistorySender readSquadMembershipHistorySender,
        ReadMemberSquadMembershipHistorySender readMemberSquadMembershipHistorySender,
        ReadSquadRequestSender readSquadRequestSender,
        DeclineSquadJoinRequestSender declineSquadJoinRequestSender,
        AcceptSquadJoinRequestSender acceptSquadJoinRequestSender,
        SendSquadJoinRequestSender sendSquadJoinRequestSender,
        ReadAllMembersSender readAllMembersSender,
        ReadAllSquadSender readAllSquadSender,
        UpdateSquadSender updateSquadSender,
        IHubContext<SquadHub> squadHub, CreateSquadSender createSquadSender, SyncChannelsSender syncChannelsSender, SyncUserSender syncUserSender, ReadMemberSquadRequestSender readMemberSquadRequestSender, WithdrawSpecializationSender withdrawSpecializationSender, ApproveSpecializationSender approveSpecializationSender, ReadAllSpecializationsSender readAllSpecializationsSender)
    {
        _acceptCandidateRequestSender = acceptCandidateRequestSender;
        _declineCandidateRequestSender = declineCandidateRequestSender;
        _registerCandidateRequestSender = registerCandidateRequestSender;
        _readCandidateRequestSender = readCandidateRequestSender;
        _sendCandidateRequestSender = sendCandidateRequestSender;
        _sendDirectMessageSender = sendDirectMessageSender;
        _sendLogMessageSender = sendLogMessageSender;
        _readMemberByIdSender = readMemberByIdSender;
        _updateMemberSender = updateMemberSender;
        _memberHub = memberHub;
        _readSquadSender = readSquadSender;
        _openSquadMembershipHistorySender = openSquadMembershipHistorySender;
        _closeSquadMembershipHistorySender = closeSquadMembershipHistorySender;
        _readSquadMembershipHistorySender = readSquadMembershipHistorySender;
        _readMemberSquadMembershipHistorySender = readMemberSquadMembershipHistorySender;
        _readSquadRequestSender = readSquadRequestSender;
        _declineSquadJoinRequestSender = declineSquadJoinRequestSender;
        _acceptSquadJoinRequestSender = acceptSquadJoinRequestSender;
        _sendSquadJoinRequestSender = sendSquadJoinRequestSender;
        _readAllMembersSender = readAllMembersSender;
        _readAllSquadSender = readAllSquadSender;
        _updateSquadSender = updateSquadSender;
        _squadHub = squadHub;
        _createSquadSender = createSquadSender;
        _syncChannelsSender = syncChannelsSender;
        _syncUserSender = syncUserSender;
        _readMemberSquadRequestSender = readMemberSquadRequestSender;
        _withdrawSpecializationSender = withdrawSpecializationSender;
        _approveSpecializationSender = approveSpecializationSender;
        _readAllSpecializationsSender = readAllSpecializationsSender;
    }

    public async Task<Member?> ReadMe() => await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);

    [AllowAnonymous]
    public async Task RegisterCandidate(CandidateRequest request)
    {
        await Task.WhenAll(
            _registerCandidateRequestSender.CallAsync(request).AsTask(),
            _sendCandidateRequestSender.CallAsync(request).AsTask()).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task AcceptCandidate(int id)
    {
        ICandidateRegisteredRequest? request = await _readCandidateRequestSender.CallAsync(id).ConfigureAwait(false);
        if (request is null) throw new ArgumentException("Request not found", nameof(id));
        if (request.AuxiliarySquad is not null) throw new ArgumentException("Auxiliary request", nameof(id));
        IMember? member = await _readMemberByIdSender.CallAsync(request.MemberId).ConfigureAwait(false);
        if (member is null) throw new ArgumentException("Member not found", nameof(id));
        await _acceptCandidateRequestSender.CallAsync(id).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), member.Id, ModelState.Updated).ConfigureAwait(false);
        await _syncUserSender.CallAsync((await _readMemberByIdSender.CallAsync(member.Id).ConfigureAwait(false))!).ConfigureAwait(false);
        if (member.DiscordId.HasValue)
            await _sendDirectMessageSender.CallAsync(new(
                member.DiscordId.Value,
                "Вы стали участником клана «Андромеда»! Добро пожаловать!")
            ).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task DeclineCandidate(int id)
    {
        ICandidateRegisteredRequest? request = await _readCandidateRequestSender.CallAsync(id).ConfigureAwait(false);
        if (request is null) throw new ArgumentException("Request not found", nameof(id));
        if (request.AuxiliarySquad is not null) throw new ArgumentException("Auxiliary request", nameof(id));
        IMember? member = await _readMemberByIdSender.CallAsync(request.MemberId).ConfigureAwait(false);
        if (member is null) throw new ArgumentException("Member not found", nameof(id));
        await _declineCandidateRequestSender.CallAsync(id).ConfigureAwait(false);
        if (member.DiscordId.HasValue)
            await _sendDirectMessageSender.CallAsync(new(
                member.DiscordId.Value,
                "Ваша заявка в клан «Андромеда» была отклонена")
            ).ConfigureAwait(false);
    }

    [Authorize]
    public async Task ChangeNickname(string newNickname)
    {
        Member? caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);
        if (caller is null) throw new ArgumentException("Member not found");
        if (string.IsNullOrWhiteSpace(newNickname) || newNickname.Length > 24) throw new ArgumentException("Invalid nickname", nameof(newNickname));
        string oldNickname = caller.Nickname;
        caller.Nickname = newNickname;
        caller = await _updateMemberSender.CallAsync(caller).ConfigureAwait(false);
        await _syncUserSender.CallAsync(caller).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), caller.Id, ModelState.Updated).ConfigureAwait(false);
        await _sendLogMessageSender.CallAsync(string.Format(Resources.CoreHub_Nickname_Message, oldNickname, newNickname)).ConfigureAwait(false);
    }

    [Authorize]
    public async Task ChangeRealName(string newRealName)
    {
        Member? caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);
        if (caller is null) throw new ArgumentException("Member not found");
        caller.RealName = newRealName;
        await _updateMemberSender.CallAsync(caller).ConfigureAwait(false);
        await _syncUserSender.CallAsync((await _readMemberByIdSender.CallAsync(caller.Id).ConfigureAwait(false))!).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), caller.Id, ModelState.Updated).ConfigureAwait(false);
    }

    [Authorize]
    public async Task ChangeTimezone(string? timezoneId)
    {
        Member? caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);
        if (caller is null) throw new ArgumentException("Member not found");
        TimeZoneInfo? timeZone = string.IsNullOrEmpty(timezoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        caller.TimeZone = timeZone;
        await _updateMemberSender.CallAsync(caller).ConfigureAwait(false);
        await _syncUserSender.CallAsync((await _readMemberByIdSender.CallAsync(caller.Id).ConfigureAwait(false))!).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), caller.Id, ModelState.Updated).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task Kick(int memberId)
    {
        Member target = await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false) ?? throw new ArgumentException();
        target = await _updateMemberSender.CallAsync(target with
        {
            IsSquadCommander = false,
            SquadNumber = null,
            SquadPartNumber = 0,
            Rank = Rank.Guest,
        }).ConfigureAwait(false);
        await _closeSquadMembershipHistorySender.CallAsync(target.Id).ConfigureAwait(false);
        await _syncUserSender.CallAsync(target).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), memberId, ModelState.Updated).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task Exile(int memberId)
    {
        Member target = await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false) ?? throw new ArgumentException();
        target = await _updateMemberSender.CallAsync(target with
        {
            IsSquadCommander = false,
            SquadNumber = null,
            SquadPartNumber = 0,
            Rank = Rank.Outcast,
        }).ConfigureAwait(false);
        await _closeSquadMembershipHistorySender.CallAsync(target.Id).ConfigureAwait(false);
        await _syncUserSender.CallAsync(target).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), memberId, ModelState.Updated).ConfigureAwait(false);
    }

    [NotInSquadAuthorize]
    public async Task CreateSquad(short? number)
    {
        Member caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false) ?? throw new ArgumentException();
        Squad squad;
        if (number is not null)
        {
            if (!await _readMemberSquadMembershipHistorySender.CallAsync(caller.Id).AnyAsync(x => x.SquadId == number).ConfigureAwait(false)) throw new InvalidOperationException();

            squad = await _readSquadSender.CallAsync(number.Value).ConfigureAwait(false);
            if (squad.IsActive) throw new InvalidOperationException();
            squad = await _updateSquadSender.CallAsync(squad with
            {
                IsActive = true,
            }).ConfigureAwait(false);
            await _squadHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), squad.Number, ModelState.Updated).ConfigureAwait(false);
        }
        else
        {
            squad = await _readSquadSender.CallAsync(await _createSquadSender.CallAsync(0).ConfigureAwait(false)).ConfigureAwait(false);
            await _squadHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), squad.Number, ModelState.Created).ConfigureAwait(false);
        }

        caller = await _updateMemberSender.CallAsync(caller with
        {
            SquadNumber = squad.Number,
            SquadPartNumber = 0,
            IsSquadCommander = true,
        }).ConfigureAwait(false);
        await _syncUserSender.CallAsync(caller).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), caller.Id, ModelState.Updated).ConfigureAwait(false);
    }

    [CaptainAuthorize]
    public async Task DisbandSquad()
    {
        Member caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false) ?? throw new ArgumentException();
        int membersCount = await _readAllMembersSender.CallAsync(0).CountAsync(x => x.SquadNumber == caller.SquadNumber).ConfigureAwait(false);
        short squadNumber = caller.SquadNumber!.Value;
        if (membersCount > 1) throw new InvalidOperationException();
        caller = await _updateMemberSender.CallAsync(caller with
        {
            SquadNumber = null,
            SquadPartNumber = 0,
            IsSquadCommander = false,
        }).ConfigureAwait(false);
        await _closeSquadMembershipHistorySender.CallAsync(caller.Id).ConfigureAwait(false);
        await foreach (SquadRequest request in _readSquadRequestSender.CallAsync(squadNumber).ConfigureAwait(false))
            await _declineSquadJoinRequestSender.CallAsync((request.SquadNumber, request.MemberId, false)).ConfigureAwait(false);
        await _updateSquadSender.CallAsync(await _readSquadSender.CallAsync(squadNumber).ConfigureAwait(false) with
        {
            IsActive = false,
        }).ConfigureAwait(false);
        await _syncUserSender.CallAsync(caller).ConfigureAwait(false);
        await _squadHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), squadNumber, ModelState.Updated).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), caller.Id, ModelState.Updated).ConfigureAwait(false);
    }

    [MinRankAuthorize]
    public async Task<ISquad> ReadSquad(short number) => await _readSquadSender.CallAsync(number).ConfigureAwait(false);

    [MinRankAuthorize]
    public IAsyncEnumerable<ISquad> ReadAllSquads() => _readAllSquadSender.CallAsync(0);

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task AppendSquadName(short number, string name)
    {
        Squad squad = await _readSquadSender.CallAsync(number).ConfigureAwait(false);
        squad.Names.Add(name);
        await _updateSquadSender.CallAsync(squad).ConfigureAwait(false);
        await _syncChannelsSender.CallAsync(new()).ConfigureAwait(false);
        await _squadHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), number, ModelState.Updated).ConfigureAwait(false);
    }

    [CaptainAuthorize]
    public async Task CreateSquadPart(int leaderId)
    {
        Member caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false) ?? throw new ArgumentException();
        int max = await _readAllMembersSender.CallAsync(0).Where(x => x.SquadNumber == caller.SquadNumber).MaxAsync(x => x.SquadPartNumber).ConfigureAwait(false);
        if (max >= short.MaxValue) throw new ArgumentOutOfRangeException(nameof(max));
        Member? leader = await _readMemberByIdSender.CallAsync(leaderId).ConfigureAwait(false);
        if (leader is null) throw new KeyNotFoundException();
        if (leader.SquadNumber == caller.SquadNumber && leader.SquadPartNumber == caller.SquadPartNumber && !leader.IsSquadCommander)
            leader = await _updateMemberSender.CallAsync(leader with
            {
                SquadPartNumber = (short)(max + 1),
                IsSquadCommander = true,
            }).ConfigureAwait(false);
        await _syncUserSender.CallAsync(leader).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), leaderId, ModelState.Updated).ConfigureAwait(false);
    }

    [CaptainAuthorize]
    public async Task MoveMemberToSquadPart(short targetSquadPart, int memberId)
    {
        Member caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false) ?? throw new ArgumentException();
        Member target = await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false) ?? throw new ArgumentException();
        if (caller.SquadNumber != target.SquadNumber) throw new InvalidOperationException();
        if (target.IsSquadCommander) throw new InvalidOperationException();
        target = await _updateMemberSender.CallAsync(target with
        {
            SquadPartNumber = targetSquadPart,
        }).ConfigureAwait(false);
        await _syncUserSender.CallAsync(target).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), memberId, ModelState.Updated).ConfigureAwait(false);
    }

    [LieutenantAuthorize]
    public async Task SetMySquadPartLeader(int memberId)
    {
        Member caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false) ?? throw new ArgumentException();
        Member newLeader = await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false) ?? throw new ArgumentException();
        if (caller.SquadNumber == newLeader.SquadNumber && caller.SquadPartNumber == newLeader.SquadPartNumber)
        {
            caller = await _updateMemberSender.CallAsync(caller with
            {
                IsSquadCommander = false,
            }).ConfigureAwait(false);
            newLeader = await _updateMemberSender.CallAsync(newLeader with
            {
                IsSquadCommander = true,
            }).ConfigureAwait(false);
            await _syncUserSender.CallAsync(caller).ConfigureAwait(false);
            await _syncUserSender.CallAsync(newLeader).ConfigureAwait(false);
            await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), caller.Id, ModelState.Updated).ConfigureAwait(false);
            await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), newLeader.Id, ModelState.Updated).ConfigureAwait(false);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public async Task SetSquadPartLeader(short targetSquadPart, int memberId)
    {
        Member caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false) ?? throw new ArgumentException();
        Member? oldLeader = await _readAllMembersSender.CallAsync(0).FirstOrDefaultAsync(x =>
            x.SquadNumber == caller.SquadNumber
            && x.SquadPartNumber == targetSquadPart
            && x.IsSquadCommander).ConfigureAwait(false);
        Member newLeader = await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false) ?? throw new ArgumentException();
        if (caller.SquadNumber == newLeader.SquadNumber && targetSquadPart == newLeader.SquadPartNumber)
        {
            if (oldLeader is not null)
            {
                oldLeader = await _updateMemberSender.CallAsync(oldLeader with
                {
                    IsSquadCommander = false,
                }).ConfigureAwait(false);
                await _syncUserSender.CallAsync(oldLeader).ConfigureAwait(false);
                await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), oldLeader.Id, ModelState.Updated).ConfigureAwait(false);
            }

            newLeader = await _updateMemberSender.CallAsync(newLeader with
            {
                IsSquadCommander = true,
            }).ConfigureAwait(false);
            await _syncUserSender.CallAsync(newLeader).ConfigureAwait(false);
            await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), newLeader.Id, ModelState.Updated).ConfigureAwait(false);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    [NotInSquadAuthorize]
    public async Task SendSquadJoinRequest(short squadNumber)
    {
        int memberId = int.Parse(Context.UserIdentifier!);
        await _sendSquadJoinRequestSender.CallAsync((memberId, squadNumber)).ConfigureAwait(false);
        await Clients.User((await _readAllMembersSender.CallAsync(0)
            .SingleAsync(x => x.IsSquadCommander && (x.SquadNumber == squadNumber) & (x.SquadPartNumber == 0))
            .ConfigureAwait(false)).Id.ToString("D")).NewSquadRequest(memberId).ConfigureAwait(false);
    }

    [CaptainAuthorize]
    public async Task AcceptSquadJoinRequest(short squadNumber, short squadPartNumber, int memberId)
    {
        await _acceptSquadJoinRequestSender.CallAsync((squadNumber, squadPartNumber, memberId)).ConfigureAwait(false);
        await _syncUserSender.CallAsync((await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false))!).ConfigureAwait(false);
        await _openSquadMembershipHistorySender.CallAsync((memberId, squadNumber)).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), memberId, ModelState.Updated).ConfigureAwait(false);
    }

    [CaptainAuthorize]
    public async Task DeclineSquadJoinRequest(short squadNumber, int memberId)
    {
        await _declineSquadJoinRequestSender.CallAsync((squadNumber, memberId, false)).ConfigureAwait(false);
        await Clients.User(memberId.ToString("D")).SquadRequestDeclined(squadNumber).ConfigureAwait(false);
    }

    [Authorize]
    public async Task CancelSquadJoinRequest(short squadNumber) => await _declineSquadJoinRequestSender.CallAsync((squadNumber, int.Parse(Context.UserIdentifier!), true)).ConfigureAwait(false);

    [SquadMemberAuthorize]
    [MinRankAuthorize]
    public IAsyncEnumerable<ISquadRequest> ReadSquadJoinRequests(short squadNumber) => _readSquadRequestSender.CallAsync(squadNumber);

    [NotInSquadAuthorize]
    public IAsyncEnumerable<ISquadRequest> ReadMySquadJoinRequests() => _readMemberSquadRequestSender.CallAsync(int.Parse(Context.UserIdentifier!));

    [LieutenantAuthorize]
    public async Task RiseAuxiliary(int memberId)
    {
        Member caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false) ?? throw new ArgumentException();
        Member? auxiliary = await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false);
        if (auxiliary is null) throw new KeyNotFoundException(nameof(memberId));
        if (auxiliary.Rank == Rank.SeniorAuxiliary) return;
        if (auxiliary.Rank != Rank.Auxiliary) throw new InvalidOperationException();
        if (caller.SquadNumber == auxiliary.SquadNumber && (caller.SquadNumber == 0 || caller.SquadPartNumber == auxiliary.SquadPartNumber))
        {
            auxiliary = await _updateMemberSender.CallAsync(auxiliary with
            {
                Rank = Rank.SeniorAuxiliary,
            }).ConfigureAwait(false);
            await _syncUserSender.CallAsync(auxiliary).ConfigureAwait(false);
            await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), auxiliary.Id, ModelState.Updated).ConfigureAwait(false);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    [LieutenantAuthorize]
    public async Task DemoteAuxiliary(int memberId)
    {
        Member caller = await _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false) ?? throw new ArgumentException();
        Member? auxiliary = await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false);
        if (auxiliary is null) throw new KeyNotFoundException(nameof(memberId));
        if (auxiliary.Rank == Rank.Auxiliary) return;
        if (auxiliary.Rank != Rank.SeniorAuxiliary) throw new InvalidOperationException();
        if (caller.SquadNumber == auxiliary.SquadNumber && (caller.SquadNumber == 0 || caller.SquadPartNumber == auxiliary.SquadPartNumber))
        {
            await _updateMemberSender.CallAsync(auxiliary with
            {
                Rank = Rank.Auxiliary,
            }).ConfigureAwait(false);
            await _syncUserSender.CallAsync(auxiliary).ConfigureAwait(false);
            await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), auxiliary.Id, ModelState.Updated).ConfigureAwait(false);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    [CaptainAuthorize]
    public async Task KickFromSquad(int memberId)
    {
        Member?[] results = await Task.WhenAll(
            _readMemberByIdSender.CallAsync(int.Parse(Context.UserIdentifier!)).AsTask(),
            _readMemberByIdSender.CallAsync(memberId).AsTask()).ConfigureAwait(false);
        if (results.Any(x => x is null)) throw new ArgumentNullException(nameof(memberId));
        if (results[0]!.SquadNumber != results[1]!.SquadNumber) throw new InvalidOperationException();
        results[1] = await _updateMemberSender.CallAsync(results[1]! with
        {
            SquadNumber = null,
            SquadPartNumber = default,
        }).ConfigureAwait(false);
        await _closeSquadMembershipHistorySender.CallAsync(memberId).ConfigureAwait(false);
        await _syncUserSender.CallAsync(results[1]!).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), memberId, ModelState.Updated).ConfigureAwait(false);
    }

    [SquadMemberAuthorize]
    public async Task LeaveFromSquad()
    {
        int memberId = int.Parse(Context.UserIdentifier!);
        Member? caller = await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false);
        if (caller is null) throw new ArgumentNullException(nameof(memberId));
        caller = await _updateMemberSender.CallAsync(caller with
        {
            SquadNumber = null,
            SquadPartNumber = default,
            Rank = caller.Rank is Rank.SeniorAuxiliary or Rank.Auxiliary ? Rank.Ally : caller.Rank,
        }).ConfigureAwait(false);
        await _closeSquadMembershipHistorySender.CallAsync(memberId).ConfigureAwait(false);
        await _syncUserSender.CallAsync(caller).ConfigureAwait(false);
        await _memberHub.Clients.All.SendAsync(nameof(IModelCrudClientMethods.ModelUpdated), memberId, ModelState.Updated).ConfigureAwait(false);
    }

    [SquadMemberAuthorize]
    [MinRankAuthorize]
    public IAsyncEnumerable<ISquadMembershipHistoryEntry> ReadSquadMembershipHistory(short squadNumber) => _readSquadMembershipHistorySender.CallAsync(squadNumber);

    [MinRankAuthorize]
    public IAsyncEnumerable<ISquadMembershipHistoryEntry> ReadMemberSquadMembershipHistory(int memberId) => _readMemberSquadMembershipHistorySender.CallAsync(memberId);
    [Authorize]
    public IAsyncEnumerable<Specialization> ReadAllSpecializations() => _readAllSpecializationsSender.CallAsync(0);
    [MinRankAuthorize(Rank.Advisor)]
    public async Task ApproveSpecialization(int memberId, int specializationId) => await _approveSpecializationSender.CallAsync((memberId, specializationId, int.Parse(Context.UserIdentifier!))).ConfigureAwait(false);
    [MinRankAuthorize(Rank.Advisor)]
    public async Task WithdrawSpecialization(int memberId, int specializationId) => await _withdrawSpecializationSender.CallAsync((memberId, specializationId, int.Parse(Context.UserIdentifier!))).ConfigureAwait(false);
}