using System.Runtime.CompilerServices;
using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.Crud;
using And9.Lib.Broker.Crud.Listener;
using And9.Service.Award.Database;
using And9.Service.Award.Senders;
using And9.Service.Award.Senders.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RabbitMQ.Client;

namespace And9.Service.Award.Listeners;

public class AwardModelCrudListener : RabbitCrudListener<Abstractions.Models.Award>, IAwardModelServiceMethods
{
    private readonly ReadCollectionListener<Abstractions.Models.Award> _readMemberListener;
    private readonly IServiceScopeFactory _scopeFactory;

    public AwardModelCrudListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithResponse<Abstractions.Models.Award, int>> createLogger,
        ILogger<BaseRabbitListenerWithResponse<int, Abstractions.Models.Award?>> readLogger,
        ILogger<BaseRabbitListenerWithStreamResponse<object, Abstractions.Models.Award>> readAllLogger,
        ILogger<BaseRabbitListenerWithResponse<Abstractions.Models.Award, Abstractions.Models.Award>> updateLogger,
        ILogger<BaseRabbitListenerWithoutResponse<int>> deleteLogger, IServiceScopeFactory scopeFactory)
        : base(connection, AwardCrudSender.QUEUE_NAME, AwardCrudSender.CRUD_FLAGS, createLogger, readLogger, readAllLogger, updateLogger, deleteLogger)
    {
        _scopeFactory = scopeFactory;
        _readMemberListener = new(connection, AwardCrudSender.QUEUE_NAME, nameof(ReadByMemberId), readAllLogger, ReadMemberExec);
    }

    public async IAsyncEnumerable<Abstractions.Models.Award> ReadByMemberId(int memberId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        AwardDataContext awardDataContext = scope.ServiceProvider.GetRequiredService<AwardDataContext>();

        await foreach (Abstractions.Models.Award award in awardDataContext.Awards
                           .Where(x => x.MemberId == memberId).AsAsyncEnumerable().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return award;
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken) => await Task.WhenAll(
        _readMemberListener.StartAsync(cancellationToken),
        base.StartAsync(cancellationToken)).ConfigureAwait(false);

    public override async Task StopAsync(CancellationToken cancellationToken) => await Task.WhenAll(
        _readMemberListener.StopAsync(cancellationToken),
        base.StopAsync(cancellationToken)).ConfigureAwait(false);

    private IAsyncEnumerable<Abstractions.Models.Award> ReadMemberExec(object arg, CancellationToken token)
    {
        if (arg is int number) return ReadByMemberId(number, token);
        string? text = arg.ToString();
        if (text is null) throw new ArgumentNullException(nameof(arg));
        return ReadByMemberId(int.Parse(text), token);
    }

    public override async Task<int> Create(Abstractions.Models.Award entity)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        AwardDataContext awardDataContext = scope.ServiceProvider.GetRequiredService<AwardDataContext>();
        RaiseMemberUpdateSender raiseMemberUpdateSender = scope.ServiceProvider.GetRequiredService<RaiseMemberUpdateSender>();
        /*Abstractions.Models.Award award = entity with
        {
            Type = entity.Type,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            AutomationTag = null,
            LastChanged = DateTime.UtcNow,
            ConcurrencyToken = Guid.NewGuid(),
        };*/
        Abstractions.Models.Award award = new()
        {
            Type = entity.Type,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            LastChanged = DateTime.UtcNow,
            ConcurrencyToken = Guid.NewGuid(),
            Description = entity.Description,
            GaveById = entity.GaveById,
            MemberId = entity.MemberId,
            AutomationTag = entity.AutomationTag,
        };
        EntityEntry<Abstractions.Models.Award> result =
            await awardDataContext.Awards.AddAsync(award).ConfigureAwait(false);
        await awardDataContext.SaveChangesAsync().ConfigureAwait(false);

        await raiseMemberUpdateSender.CallAsync(new()
        {
            MemberId = award.MemberId,
            Points = await awardDataContext.GetPointsAsync(result.Entity.MemberId).ConfigureAwait(false),
        }).ConfigureAwait(false);
        return result.Entity.Id;
    }

    public override Task<Abstractions.Models.Award> Update(Abstractions.Models.Award entity) => throw new NotImplementedException();

    public override async Task<Abstractions.Models.Award?> Read(int id)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        AwardDataContext awardDataContext = scope.ServiceProvider.GetRequiredService<AwardDataContext>();
        return await awardDataContext.Awards.FindAsync(id).ConfigureAwait(false);
    }

    public override async IAsyncEnumerable<Abstractions.Models.Award> ReadAll(object _, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable __ = scope.ConfigureAwait(false);
        AwardDataContext awardDataContext = scope.ServiceProvider.GetRequiredService<AwardDataContext>();
        await foreach (Abstractions.Models.Award award in awardDataContext.Awards)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return award;
        }
    }

    public override Task Delete(int id) => throw new NotSupportedException();
}