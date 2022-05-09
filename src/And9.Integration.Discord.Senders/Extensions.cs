using And9.Integration.Discord.Abstractions.Interfaces;
using And9.Integration.Discord.Senders.Models;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;

namespace And9.Integration.Discord.Senders;

public static class Extensions
{
    public static BrokerBuilder AddDiscordSenders(this BrokerBuilder builder)
    {
        builder
            .AppendSenderWithoutResponse<SendDirectMessageSender, SendDirectMessageArg>()
            .AppendSenderWithoutResponse<SyncUserSender, Member>()
            .AppendSenderWithoutResponse<SyncRolesSender, object>()
            .AppendSenderWithoutResponse<SyncChannelsSender, object>()
            .AppendSenderWithoutResponse<SendLogMessageSender, string>()
            .AppendSenderWithoutResponse<SendCandidateRequestSender, ICandidateRequest>()
            .AppendSenderWithResponse<ResolveDiscordUserNameSender, string, ulong?>()
            .AppendSenderWithResponse<RegisterChannelSender, IChannel, bool>()
            .AppendSenderWithResponse<RegisterChannelCategorySender, ulong, bool>();
        return builder;
    }
}