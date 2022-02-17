using And9.Lib.Broker;
using And9.Service.Auth.Abstractions.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Auth.Senders;

public class LoginSender : BaseRabbitSenderWithResponse<AuthCredentials, string>
{
    public const string QUEUE_NAME = "And9.Service.Auth.Login";
    public LoginSender(IConnection connection, ILogger<LoginSender> logger) : base(connection, QUEUE_NAME, logger) { }
}