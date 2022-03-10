using And9.Lib.Broker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Election.Senders;

public class CancelRegisterSender : BaseRabbitSenderWithResponse<int, bool>
{
    public const string QUEUE_NAME = "And9.Service.Election.CancelRegister";
    public CancelRegisterSender(IConnection connection, ILogger<CancelRegisterSender> logger) : base(connection, QUEUE_NAME, logger) { }
}