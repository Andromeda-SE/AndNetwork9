using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Election.Senders;

public class RegisterSender : BaseRabbitSenderWithResponse<int, bool>
{
    public const string QUEUE_NAME = "And9.Service.Election.Register";
    public RegisterSender(IConnection connection, ILogger<RegisterSender> logger) : base(connection, QUEUE_NAME, logger) { }
}