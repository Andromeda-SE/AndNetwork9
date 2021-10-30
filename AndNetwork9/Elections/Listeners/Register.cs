using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Elections;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Backend.Senders.Elections;
using AndNetwork9.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Elections.Listeners;

public class Register : BaseRabbitListenerWithoutResponse<int>
{
    private readonly RewriteElectionsChannelSender _rewriteElectionsChannelSender;
    private readonly IServiceScopeFactory _scopeFactory;

    public Register(IConnection connection, RewriteElectionsChannelSender rewriteElectionsChannelSender,
        IServiceScopeFactory scopeFactory, ILogger<Register> logger) : base(connection,
        RegisterSender.QUEUE_NAME,
        logger)
    {
        _rewriteElectionsChannelSender = rewriteElectionsChannelSender;
        _scopeFactory = scopeFactory;
    }

    public override async Task Run(int memberId)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        ClanDataContext data = (ClanDataContext)scope.ServiceProvider.GetService(typeof(ClanDataContext))!;
        if (data is null) throw new ApplicationException();

        Member? member = await data.Members.FindAsync(memberId).ConfigureAwait(false);

        if (member is null) throw new FailedCallException(HttpStatusCode.NotFound);
        if (member.Rank < Rank.Assistant || member.Direction <= Direction.None)
            throw new FailedCallException(HttpStatusCode.Forbidden);

        Election? election;
        try
        {
            election = await data.Elections.SingleAsync(x => x.Stage == ElectionStage.Registration)
                .ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
            throw new FailedCallException(HttpStatusCode.Gone);
        }

        if (election.Votings.SelectMany(x => x.Members).Any(x => x.MemberId == memberId))
            throw new FailedCallException(HttpStatusCode.AlreadyReported);
        ElectionVoting voting = election.Votings.Single(x => x.Direction == member.Direction);
        voting.Members.Add(new()
        {
            Direction = member.Direction,
            ElectionId = election.Id,
            MemberId = memberId,
            Votes = 0,
            Voted = false,
        });
        await data.SaveChangesAsync().ConfigureAwait(false);
        await _rewriteElectionsChannelSender.CallAsync(election).ConfigureAwait(false);
    }
}