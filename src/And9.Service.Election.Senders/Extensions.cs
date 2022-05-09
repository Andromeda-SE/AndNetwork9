using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Service.Election.Senders;

public static class Extensions
{
    public static BrokerBuilder AddElectionSenders(this BrokerBuilder builder)
    {
        builder.AppendSenderWithResponse<VoteSender, (int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes), bool>();
        builder.AppendSenderWithResponse<RegisterSender, int, bool>();
        builder.AppendSenderWithCollectionResponse<CurrentElectionSender, int, Abstractions.Models.Election>();
        builder.AppendSenderWithResponse<CancelRegisterSender, int, bool>();
        return builder;
    }
}