using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using RabbitMQ.Client;

namespace And9.Service.Core.Listeners;

public class ReadMemberByNicknameListener : BaseRabbitListenerWithResponse<string, Member?>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReadMemberByNicknameListener(IConnection connection, ILogger<ReadMemberByNicknameListener> logger, IServiceScopeFactory scopeFactory)
        : base(connection, ReadMemberByNicknameSender.QUEUE_NAME, logger) => _scopeFactory = scopeFactory;

    protected override async Task<Member?> GetResponseAsync(string request)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        CoreDataContext coreDataContext = scope.ServiceProvider.GetRequiredService<CoreDataContext>();

        return coreDataContext.Members.FirstOrDefault(x => x.Nickname == request);
    }
}