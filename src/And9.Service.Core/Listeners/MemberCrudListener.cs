using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Lib.Broker.Crud;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RabbitMQ.Client;

namespace And9.Service.Core.Listeners;

public class MemberCrudListener : RabbitCrudListener<Member>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MemberCrudListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithResponse<Member, int>> createLogger,
        ILogger<BaseRabbitListenerWithResponse<int, Member?>> readLogger,
        ILogger<BaseRabbitListenerWithStreamResponse<object, Member>> readCollectionLogger,
        ILogger<BaseRabbitListenerWithResponse<Member, Member>> updateLogger,
        ILogger<BaseRabbitListenerWithoutResponse<int>> deleteLogger,
        IServiceScopeFactory scopeFactory)
        : base(connection, MemberCrudSender.QUEUE_NAME, MemberCrudSender.CRUD_FLAGS, createLogger, readLogger, readCollectionLogger, updateLogger, deleteLogger) => _scopeFactory = scopeFactory;

    public override async Task<int> Create(Member entity)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        EntityEntry<Member> result = await coreDataContext.Members.AddAsync(new()
        {
            Direction = entity.Direction,
            DiscordId = entity.DiscordId,
            IsSquadCommander = entity.IsSquadCommander,
            JoinDate = entity.JoinDate,
            LastDirectionChange = DateOnly.FromDateTime(DateTime.UtcNow),
            MicrosoftId = entity.MicrosoftId,
            Nickname = entity.Nickname,
            Rank = entity.Rank,
            RealName = entity.RealName,
            SquadNumber = entity.SquadNumber,
            SquadPartNumber = entity.SquadPartNumber,
            SteamId = entity.SteamId,
            TelegramId = entity.TelegramId,
            TimeZone = entity.TimeZone,
            VkId = entity.VkId,
        }).ConfigureAwait(false);
        await coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        return result.Entity.Id;
    }

    public override async Task<Member> Update(Member entity)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        Member? member = await coreDataContext.Members.FindAsync(entity.Id).ConfigureAwait(false);
        if (member is null) throw new ArgumentException("Member not found", nameof(entity));

        if (member.Direction != entity.Direction)
        {
            member.Direction = entity.Direction;
            member.LastDirectionChange = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        if (member.DiscordId != entity.DiscordId) member.DiscordId = entity.DiscordId;
        if (member.IsSquadCommander != entity.IsSquadCommander) member.IsSquadCommander = entity.IsSquadCommander;
        if (member.JoinDate != entity.JoinDate) member.JoinDate = entity.JoinDate;
        if (member.MicrosoftId != entity.MicrosoftId) member.MicrosoftId = entity.MicrosoftId;
        if (member.Nickname != entity.Nickname) member.Nickname = entity.Nickname;
        if (member.Rank != entity.Rank) member.Rank = entity.Rank;
        if (member.RealName != entity.RealName) member.RealName = entity.RealName;
        if (member.SquadNumber != entity.SquadNumber) member.SquadNumber = entity.SquadNumber;
        if (member.SquadPartNumber != entity.SquadPartNumber) member.SquadPartNumber = entity.SquadPartNumber;
        if (member.SteamId != entity.SteamId) member.SteamId = entity.SteamId;
        if (member.TelegramId != entity.TelegramId) member.TelegramId = entity.TelegramId;
        if (member.TimeZone?.Id != entity.TimeZone?.Id) member.TimeZone = entity.TimeZone;
        if (member.VkId != entity.VkId) member.VkId = entity.VkId;

        await coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        return member;
    }

    public override async Task<Member?> Read(int id)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        Member? member = await coreDataContext.Members.FindAsync(id).ConfigureAwait(false);
        if (member is null) throw new ArgumentException("Member not found", nameof(id));
        return member;
    }

    public override async IAsyncEnumerable<Member> ReadAll(object _, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable __ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        foreach (Member member in coreDataContext.Members.Where(x => x.Rank >= Rank.Auxiliary))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return member;
        }
    }

    public override Task Delete(int id) => throw new NotSupportedException();
}