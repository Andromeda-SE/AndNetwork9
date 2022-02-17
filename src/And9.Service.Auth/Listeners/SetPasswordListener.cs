using And9.Lib.Broker;
using And9.Service.Auth.Database;
using And9.Service.Auth.Database.Models;
using And9.Service.Auth.Senders;
using RabbitMQ.Client;

namespace And9.Service.Auth.Listeners;

public class SetPasswordListener : BaseRabbitListenerWithoutResponse<(int memberId, string newPassword)>
{
    private readonly AuthDataContext _authDataContext;

    public SetPasswordListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithoutResponse<(int memberId, string newPassword)>> logger,
        AuthDataContext authDataContext)
        : base(connection, SetPasswordSender.QUEUE_NAME, logger) => _authDataContext = authDataContext;

    public override async Task Run((int memberId, string newPassword) request)
    {
        (int memberId, string newPassword) = request;
        PasswordHash? hash = await _authDataContext.PasswordHashes.FindAsync(memberId).ConfigureAwait(false);
        if (hash is null) throw new ArgumentException(string.Empty, nameof(request));
        hash.SetPassword(newPassword);
    }
}