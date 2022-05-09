using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Auth.Database;
using And9.Service.Auth.Database.Models;
using And9.Service.Auth.Senders;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace And9.Service.Auth.ConsumerStrategies;

[QueueName(GeneratePasswordSender.QUEUE_NAME)]
public class GeneratePasswordConsumer : IBrokerConsumerWithResponseStrategy<int, string>
{
    private readonly AuthDataContext _authDataContext;

    public GeneratePasswordConsumer(AuthDataContext authDataContext) => _authDataContext = authDataContext;

    public async ValueTask<string> ExecuteAsync(int memberId)
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