using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RabbitMQ.Client;

namespace And9.Service.Core.Listeners;

public class RegisterCandidateRequestListener : BaseRabbitListenerWithResponse<CandidateRequest, int>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RegisterCandidateRequestListener(IConnection connection, ILogger<BaseRabbitListenerWithResponse<CandidateRequest, int>> logger, IServiceScopeFactory scopeFactory)
        : base(connection, RegisterCandidateRequestSender.QUEUE_NAME, logger) => _scopeFactory = scopeFactory;

    protected override async Task<int> GetResponseAsync(CandidateRequest request)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        Member member;
        Member[] members = await coreDataContext.Members.Where(x =>
            x.Nickname == request.Nickname
            || x.DiscordId == request.DiscordId
            || x.SteamId == request.SteamId
            || request.VkId != null && x.VkId == request.VkId).ToArrayAsync().ConfigureAwait(false);
        switch (members.Length)
        {
            case 0:
            {
                EntityEntry<Member> memberEntry = await coreDataContext.Members.AddAsync(new()
                {
                    Direction = Direction.None,
                    DiscordId = request.DiscordId,
                    IsSquadCommander = false,
                    JoinDate = DateOnly.MaxValue,
                    LastDirectionChange = DateOnly.MinValue,
                    MicrosoftId = null,
                    Nickname = request.Nickname,
                    Rank = request.AuxiliarySquad is null ? Rank.Candidate : Rank.Guest,
                    RealName = request.RealName,
                    SquadNumber = null,
                    SquadPartNumber = 0,
                    SteamId = request.SteamId,
                    TelegramId = null,
                    TimeZone = request.TimeZone,
                    VkId = request.VkId,
                }).ConfigureAwait(false);
                member = memberEntry.Entity;
                break;
            }
            case 1:
            {
                member = members.Single();
                member.Nickname = request.Nickname;
                if (member.SteamId != request.SteamId || member.DiscordId != request.DiscordId) throw new ArgumentException("Conflict in accounts id's", nameof(request));
                if (member.Rank > Rank.None) throw new ArgumentException("Member in clan", nameof(request));
                break;
            }
            default:
                throw new ArgumentException("Conflict", nameof(request));
        }


        await coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        EntityEntry<CandidateRegisteredRequest> requestResult = await coreDataContext.CandidateRequests.AddAsync(new()
        {
            Accepted = null,
            Age = request.Age,
            AuxiliarySquad = request.AuxiliarySquad,
            Description = request.Description,
            HoursCount = request.HoursCount,
            Member = member,
            MemberId = member.Id,
            Recommendation = request.Recommendation,
        }).ConfigureAwait(false);
        await coreDataContext.SaveChangesAsync().ConfigureAwait(false);
        return requestResult.Entity.Id;
    }
}