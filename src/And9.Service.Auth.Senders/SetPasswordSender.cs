using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Auth.Senders;

public class SetPasswordSender : BaseRabbitSenderWithoutResponse<(int memberId, string newPassword)>
{
    public const string QUEUE_NAME = "And9.Service.Auth.SetPassword";
    public SetPasswordSender(IConnection connection, ILogger<BaseRabbitSenderWithoutResponse<(int memberId, string newPassword)>> logger) : base(connection, QUEUE_NAME, logger) { }
}