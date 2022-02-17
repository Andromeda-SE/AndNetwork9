using And9.Lib.Broker;
using And9.Service.Auth.Database;
using And9.Service.Auth.Database.Models;
using And9.Service.Auth.Senders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RabbitMQ.Client;

namespace And9.Service.Auth.Listeners;

public class GeneratePasswordListener : BaseRabbitListenerWithResponse<int, string>
{
    private readonly AuthDataContext _authDataContext;

    public GeneratePasswordListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithResponse<int, string>> logger,
        AuthDataContext authDataContext)
        : base(connection, GeneratePasswordSender.QUEUE_NAME, logger) => _authDataContext = authDataContext;

    protected override async Task<string> GetResponseAsync(int memberId)
    {
        PasswordHash? hash = await _authDataContext.PasswordHashes.FindAsync(memberId).ConfigureAwait(false);
        if (hash is null)
        {
            EntityEntry<PasswordHash> entry = await _authDataContext.PasswordHashes.AddAsync(new()
            {
                Hash = Array.Empty<byte>(),
                UserId = memberId,
            }).ConfigureAwait(false);
            hash = entry.Entity;
        }

        string result = hash.SetRandomPassword();
        await _authDataContext.SaveChangesAsync().ConfigureAwait(false);
        return result;
    }
}