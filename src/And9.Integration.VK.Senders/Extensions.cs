using And9.Lib.Broker;

namespace And9.Integration.VK.Senders;

public static class Extensions
{
    public static BrokerBuilder AddVkSenders(this BrokerBuilder builder)
    {
        builder.AppendSenderWithoutResponse<WallPublishSender, string>();
        builder.AppendSenderWithResponse<ResolveVkUrlSender, string, long?>();
        return builder;
    }
}