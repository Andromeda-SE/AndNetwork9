using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Auth.Senders;

[QueueName(QUEUE_NAME)]
public class SetPasswordSender : BrokerSenderWithoutResponse<(int memberId, string newPassword)>
{
    public const string QUEUE_NAME = "And9.Service.Auth.SetPassword";
    public SetPasswordSender(BrokerManager brokerManager) : base(brokerManager) { }
}