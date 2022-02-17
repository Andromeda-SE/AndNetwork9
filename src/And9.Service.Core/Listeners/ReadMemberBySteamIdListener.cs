using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using And9.Service.Core.Senders.Interfaces;
using RabbitMQ.Client;

namespace And9.Service.Core.Listeners;

public class ReadMemberBySteamIdListener : BaseRabbitListenerWithResponse<ulong, Member?>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReadMemberBySteamIdListener(IConnection connection, ILogger<BaseRabbitListenerWithResponse<ulong, Member?>> logger, IServiceScopeFactory scopeFactory)
        : base(connection, ReadMemberBySteamIdSender.QUEUE_NAME, logger) => _scopeFactory = scopeFactory;

    protected override async Task<Member?> GetResponseAsync(ulong request)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        return coreDataContext.Members.FirstOrDefault(x => x.SteamId == request);
    }
}