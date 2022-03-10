using System.Runtime.CompilerServices;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Models;
using And9.Service.Election.API.Interfaces;
using And9.Service.Election.Senders;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.Hubs;

public class ElectionHub : Hub<IElectionClientMethods>, IElectionServerMethods
{
    private readonly CancelRegisterSender _cancelRegisterSender;
    private readonly CurrentElectionSender _currentElectionSender;
    private readonly RegisterSender _registerSender;
    private readonly VoteSender _voteSender;

    public ElectionHub(RegisterSender registerSender, CancelRegisterSender cancelRegisterSender, VoteSender voteSender, CurrentElectionSender currentElectionSender)
    {
        _registerSender = registerSender;
        _cancelRegisterSender = cancelRegisterSender;
        _voteSender = voteSender;
        _currentElectionSender = currentElectionSender;
    }

    public async Task<bool> Register() => await _registerSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);

    public async Task<bool> CancelRegister() => await _cancelRegisterSender.CallAsync(int.Parse(Context.UserIdentifier!)).ConfigureAwait(false);

    public async Task<bool> Vote(IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> votes)
        => await _voteSender.CallAsync((int.Parse(Context.UserIdentifier!), votes)).ConfigureAwait(false);

    public async IAsyncEnumerable<Election> GetElection([EnumeratorCancellation] CancellationToken token)
    {
        await foreach (Election election in _currentElectionSender.CallAsync(0).ConfigureAwait(false)) yield return election;
    }
}