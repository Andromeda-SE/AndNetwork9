using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Integration.Steam.Senders;

[QueueName(QUEUE_NAME)]
public class ResolveSteamUrlSender : BrokerSenderWithResponse<string, ulong?>
{
    public const string QUEUE_NAME = "And9.Integration.Steam.ResolveSteamUrl";
    public ResolveSteamUrlSender(BrokerManager brokerManager) : base(brokerManager) { }
}