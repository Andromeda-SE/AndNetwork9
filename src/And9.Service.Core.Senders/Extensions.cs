using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Interfaces;
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

        builder.AppendSenderWithoutResponse<AcceptSquadJoinRequestSender, (short number, int memberId)>();
        builder.AppendSenderWithoutResponse<AppendSquadNameSender, (short number, string name)>();
        builder.AppendSenderWithoutResponse<CreateSquadPartSender, (short squadNumber, int leaderId)>();
        builder.AppendSenderWithoutResponse<CreateSquadSender, (short? sqauadNumber, int leaderId)>();
        builder.AppendSenderWithoutResponse<DeclineSquadJoinRequestSender, (short number, int memberId)>();
        builder.AppendSenderWithoutResponse<DisbandSquadSender, short>();
        builder.AppendSenderWithoutResponse<KickFromSquadSender, int>();
        builder.AppendSenderWithoutResponse<MoveMemberToSquadPartSender, (int memberId, short squadPart)>();
        builder.AppendSenderWithCollectionResponse<ReadAllSquadSender, int, ISquad>();
        builder.AppendSenderWithCollectionResponse<ReadMemberSquadMembershipHistorySender, int, ISquadMembershipHistoryEntry>();
        builder.AppendSenderWithCollectionResponse<ReadSquadMembershipHistorySender, short, ISquadMembershipHistoryEntry>();
        builder.AppendSenderWithResponse<ReadSquadSender, short, ISquad>();
        builder.AppendSenderWithoutResponse<SendSquadJoinRequestSender, (int memberId, short squadNumber)>();
        builder.AppendSenderWithoutResponse<SetSquadPartLeaderSender, (int memberId, short squadNumber, short squadPartNumber)>();

        return builder;
    }
}