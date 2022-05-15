using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Core.Senders;

public class MoveMemberToSquadPartSender : BrokerSenderWithoutResponse<(int memberId, short squadPart)>
{
    public const string QUEUE_NAME = "And9.Service.Core.MoveMemberToSquadPart";
    public MoveMemberToSquadPartSender(BrokerManager brokerManager) : base(brokerManager) { }
}