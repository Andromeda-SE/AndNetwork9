using And9.Lib.Broker;
using And9.Lib.Broker.Senders;
using And9.Service.Auth.Abstractions.Models;

namespace And9.Service.Auth.Senders;

[QueueName(QUEUE_NAME)]
public class LoginSender : BrokerSenderWithResponse<AuthCredentials, string>
{
    public const string QUEUE_NAME = "And9.Service.Auth.Login";

    public LoginSender(BrokerManager brokerManager) : base(brokerManager) { }
}