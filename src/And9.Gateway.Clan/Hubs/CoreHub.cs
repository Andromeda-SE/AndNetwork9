﻿using And9.Gateway.Clan.Auth.Attributes;
using And9.Gateway.Clan.Properties;
using And9.Integration.Discord.Senders;
using And9.Service.Core.Abstractions;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.API.Interfaces;
using And9.Service.Core.Senders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.Hubs;

public class CoreHub : Hub<ICoreClientMethods>, ICoreServerMethods
{
    private readonly AcceptCandidateRequestSender _acceptCandidateRequestSender;
    private readonly CandidateRequestCrudSender _candidateRequestCrudSender;
    private readonly DeclineCandidateRequestSender _declineCandidateRequestSender;
    private readonly MemberCrudSender _memberCrudSender;
    private readonly RegisterCandidateRequestSender _registerCandidateRequestSender;

    private readonly SendCandidateRequestSender _sendCandidateRequestSender;
    private readonly SendDirectMessageSender _sendDirectMessageSender;
    private readonly SendLogMessageSender _sendLogMessageSender;

    public CoreHub(
        RegisterCandidateRequestSender registerCandidateRequestSender,
        AcceptCandidateRequestSender acceptCandidateRequestSender,
        DeclineCandidateRequestSender declineCandidateRequestSender,
        MemberCrudSender memberCrudSender,
        SendCandidateRequestSender sendCandidateRequestSender,
        SendDirectMessageSender sendDirectMessageSender,
        CandidateRequestCrudSender candidateRequestCrudSender,
        SendLogMessageSender sendLogMessageSender)
    {
        _registerCandidateRequestSender = registerCandidateRequestSender;
        _acceptCandidateRequestSender = acceptCandidateRequestSender;
        _declineCandidateRequestSender = declineCandidateRequestSender;
        _memberCrudSender = memberCrudSender;
        _sendCandidateRequestSender = sendCandidateRequestSender;
        _sendDirectMessageSender = sendDirectMessageSender;
        _candidateRequestCrudSender = candidateRequestCrudSender;
        _sendLogMessageSender = sendLogMessageSender;
    }

    public async Task<Member?> ReadMe() => await _memberCrudSender.Read(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);

    [AllowAnonymous]
    public async Task RegisterCandidate(CandidateRequest request)
    {
        await Task.WhenAll(
            _registerCandidateRequestSender.CallAsync(request),
            _sendCandidateRequestSender.CallAsync(request)).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task AcceptCandidate(int id)
    {
        ICandidateRegisteredRequest? request = await _candidateRequestCrudSender.Read(id).ConfigureAwait(false);
        if (request is null) throw new ArgumentException("Request not found", nameof(id));
        if (request.AuxiliarySquad is not null) throw new ArgumentException("Auxiliary request", nameof(id));
        IMember? member = await _memberCrudSender.Read(request.MemberId).ConfigureAwait(false);
        if (member is null) throw new ArgumentException("Member not found", nameof(id));
        await _acceptCandidateRequestSender.CallAsync(id).ConfigureAwait(false);
        if (member.DiscordId.HasValue)
            await _sendDirectMessageSender.CallAsync(new(
                member.DiscordId.Value,
                "Вы стали учасником клана «Андромеда»! Добро пожаловать!")
            ).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task DeclineCandidate(int id)
    {
        ICandidateRegisteredRequest? request = await _candidateRequestCrudSender.Read(id).ConfigureAwait(false);
        if (request is null) throw new ArgumentException("Request not found", nameof(id));
        if (request.AuxiliarySquad is not null) throw new ArgumentException("Auxiliary request", nameof(id));
        IMember? member = await _memberCrudSender.Read(request.MemberId).ConfigureAwait(false);
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
        Member? caller = await _memberCrudSender.Read(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);
        if (caller is null) throw new ArgumentException("Member not found");
        if (string.IsNullOrWhiteSpace(newNickname) || newNickname.Length > 24) throw new ArgumentException("Invalid nickname", nameof(newNickname));
        string oldNickname = caller.Nickname;
        caller.Nickname = newNickname;
        await _memberCrudSender.Update(caller).ConfigureAwait(false);
        await _sendLogMessageSender.CallAsync(string.Format(Resources.CoreHub_Nickname_Message, oldNickname, newNickname)).ConfigureAwait(false);
    }

    [Authorize]
    public async Task ChangeRealName(string newRealName)
    {
        Member? caller = await _memberCrudSender.Read(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);
        if (caller is null) throw new ArgumentException("Member not found");
        caller.RealName = newRealName;
        await _memberCrudSender.Update(caller).ConfigureAwait(false);
    }

    [Authorize]
    public async Task ChangeDirection(Direction direction)
    {
        Member? caller = await _memberCrudSender.Read(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);
        if (caller is null) throw new ArgumentException("Member not found");
        if (caller.LastDirectionChange.AddDays(14) > DateOnly.FromDateTime(DateTime.UtcNow)) throw new ArgumentException("Dircetion change not allowed");
        caller.Direction = direction;
        await _memberCrudSender.Update(caller).ConfigureAwait(false);
        await _sendLogMessageSender.CallAsync(string.Format("Игрок **{0}** сменил направление на «{1}»", caller.Nickname, direction.GetDisplayString())).ConfigureAwait(false);
    }

    [Authorize]
    public async Task ChangeTimezone(string? timezoneId)
    {
        Member? caller = await _memberCrudSender.Read(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);
        if (caller is null) throw new ArgumentException("Member not found");
        TimeZoneInfo? timeZone = string.IsNullOrEmpty(timezoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        caller.TimeZone = timeZone;
        await _memberCrudSender.Update(caller).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public Task Kick(int memberId) => throw new NotImplementedException();

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public Task Exile(int memberId) => throw new NotImplementedException();
}