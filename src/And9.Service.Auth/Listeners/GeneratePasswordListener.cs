using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Service.Auth.Database;
using And9.Service.Auth.Database.Models;
using And9.Service.Auth.Senders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RabbitMQ.Client;

namespace And9.Service.Auth.Listeners;

public class GeneratePasswordListener : BaseRabbitListenerWithResponse<int, string>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GeneratePasswordListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithResponse<int, string>> logger,
        IServiceScopeFactory serviceScopeFactory)
        : base(connection, GeneratePasswordSender.QUEUE_NAME, logger) => _serviceScopeFactory = serviceScopeFactory;

    protected override async Task<string> GetResponseAsync(int memberId)
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        AuthDataContext authDataContext = scope.ServiceProvider.GetRequiredService<AuthDataContext>();

        PasswordHash? hash = await authDataContext.PasswordHashes.FindAsync(memberId).ConfigureAwait(false);
        if (hash is null)
        {
            EntityEntry<PasswordHash> entry = await authDataContext.PasswordHashes.AddAsync(new()
            {
                Hash = Array.Empty<byte>(),
                UserId = memberId,
            }).ConfigureAwait(false);
            hash = entry.Entity;
        }

        string result = hash.SetRandomPassword();
        await authDataContext.SaveChangesAsync().ConfigureAwait(false);
        return result;
    }
}