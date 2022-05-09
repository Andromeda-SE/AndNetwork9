using And9.Integration.Steam.Senders.Models;
using And9.Lib.Broker;

namespace And9.Integration.Steam.Senders;

public static class Extensions
{
    public static BrokerBuilder AddSteamSenders(this BrokerBuilder builder)
    {
        builder.AppendSenderWithResponse<PlayerActivitySender, ulong[], PlayerActivityResultNode[]>();
        builder.AppendSenderWithResponse<ResolveSteamUrlSender, string, ulong?>();
        return builder;
    }
}