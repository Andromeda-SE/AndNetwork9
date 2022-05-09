using And9.Gateway.Clan.Senders.Models;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Gateway.Clan.Senders;

public static class Extensions
{
    public static BrokerBuilder AddGatewaySenders(this BrokerBuilder builder)
    {
        builder.AppendSenderWithoutResponse<RaiseElectionUpdateSender, int>();
        builder.AppendSenderWithResponse<RaiseMemberUpdateSender, RaiseMemberUpdateArg, Rank>();
        return builder;
    }
}