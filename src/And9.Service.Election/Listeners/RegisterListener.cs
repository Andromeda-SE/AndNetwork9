using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Database;
using And9.Service.Election.Database.Models;
using And9.Service.Election.Senders;
using RabbitMQ.Client;

namespace And9.Service.Election.Listeners;

public class RegisterListener : BaseRabbitListenerWithResponse<int, bool>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public RegisterListener(IConnection connection, ILogger<RegisterListener> logger, IServiceScopeFactory serviceScopeFactory)
        : base(connection, RegisterSender.QUEUE_NAME, logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<bool> GetResponseAsync(int request)
    {
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        ElectionDataContext context = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();

        MemberCrudSender memberCrudSender = scope.ServiceProvider.GetRequiredService<MemberCrudSender>();
        Member? member = await memberCrudSender.Read(request).ConfigureAwait(false);
        if (member?.Rank is null or <= Rank.None or Rank.FirstAdvisor) return false;
        if (member.Direction <= Direction.None) return false;

        (short id, ElectionStatus status) = await context.GetCurrentElectionStatusAsync().ConfigureAwait(false);
        if (status is not ElectionStatus.Registration) return false;
        if (context.ElectionVotes.Any(x => x.MemberId == request)) return false;

        context.ElectionVotes.Add(new()
        {
            Direction = member.Direction,
            ElectionId = id,
            MemberId = member.Id,
            Voted = null,
            Votes = 0,
        });
        await context.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }
}