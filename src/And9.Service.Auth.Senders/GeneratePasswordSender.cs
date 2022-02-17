using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Auth.Senders;

public class GeneratePasswordSender : BaseRabbitSenderWithResponse<int, string>
{
    public const string QUEUE_NAME = "And9.Service.Auth.GeneratePassword";
    public GeneratePasswordSender(IConnection connection, ILogger<GeneratePasswordSender> logger) : base(connection, QUEUE_NAME, logger) { }
}