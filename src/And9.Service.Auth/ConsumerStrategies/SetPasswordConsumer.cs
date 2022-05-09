using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Auth.Database;
using And9.Service.Auth.Database.Models;
using And9.Service.Auth.Senders;

namespace And9.Service.Auth.ConsumerStrategies;

[QueueName(SetPasswordSender.QUEUE_NAME)]
public class SetPasswordConsumer : IBrokerConsumerWithoutResponseStrategy<(int memberId, string newPassword)>
{
    private readonly AuthDataContext _authDataContext;

    public SetPasswordConsumer(AuthDataContext authDataContext) => _authDataContext = authDataContext;

    public async ValueTask ExecuteAsync((int memberId, string newPassword) request)
    {
        (int memberId, string newPassword) = request;
        PasswordHash? hash = await _authDataContext.PasswordHashes.FindAsync(memberId).ConfigureAwait(false);
        if (hash is null) throw new ArgumentException(string.Empty, nameof(request));
        hash.SetPassword(newPassword);
    }
}