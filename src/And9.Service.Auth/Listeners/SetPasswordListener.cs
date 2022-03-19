using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Service.Auth.Database;
using And9.Service.Auth.Database.Models;
using And9.Service.Auth.Senders;
using RabbitMQ.Client;

namespace And9.Service.Auth.Listeners;

public class SetPasswordListener : BaseRabbitListenerWithoutResponse<(int memberId, string newPassword)>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SetPasswordListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithoutResponse<(int memberId, string newPassword)>> logger,
        IServiceScopeFactory serviceScopeFactory)
        : base(connection, SetPasswordSender.QUEUE_NAME, logger) => _serviceScopeFactory = serviceScopeFactory;

    public override async Task Run((int memberId, string newPassword) request)
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        AuthDataContext authDataContext = scope.ServiceProvider.GetRequiredService<AuthDataContext>();

        (int memberId, string newPassword) = request;
        PasswordHash? hash = await authDataContext.PasswordHashes.FindAsync(memberId).ConfigureAwait(false);
        if (hash is null) throw new ArgumentException(string.Empty, nameof(request));
        hash.SetPassword(newPassword);
    }
}