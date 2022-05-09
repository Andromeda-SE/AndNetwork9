using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.Senders;

public static class Extensions
{
    public static BrokerBuilder AddCoreSenders(this BrokerBuilder builder)
    {
        builder.AppendSenderWithoutResponse<AcceptCandidateRequestSender, int>();
        builder.AppendSenderWithResponse<CreateMemberSender, Member, int>();
        builder.AppendSenderWithoutResponse<DeclineCandidateRequestSender, int>();
        builder.AppendSenderWithCollectionResponse<ReadAllCandidateRequestSender, int, CandidateRegisteredRequest>();
        builder.AppendSenderWithCollectionResponse<ReadAllMembersSender, int, Member>();
        builder.AppendSenderWithResponse<ReadCandidateRequestSender, int, CandidateRegisteredRequest?>();
        builder.AppendSenderWithResponse<ReadMemberByDiscordIdSender, ulong, Member?>();
        builder.AppendSenderWithResponse<ReadMemberByIdSender, int, Member?>();
        builder.AppendSenderWithResponse<ReadMemberByNicknameSender, string, Member?>();
        builder.AppendSenderWithResponse<ReadMemberBySteamIdSender, ulong, Member?>();
        builder.AppendSenderWithResponse<RegisterCandidateRequestSender, CandidateRequest, int>();
        builder.AppendSenderWithResponse<UpdateMemberSender, Member, Member>();
        return builder;
    }
}