using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders.CandidateRequest;
using And9.Service.Core.Senders.Member;
using And9.Service.Core.Senders.Squad;
using And9.Service.Core.Senders.Squad.SquadMembershipHistory;
using And9.Service.Core.Senders.Squad.SquadRequest;

namespace And9.Service.Core.Senders;

public static class Extensions
{
    public static BrokerBuilder AddCoreSenders(this BrokerBuilder builder)
    {
        builder.AppendSenderWithoutResponse<AcceptCandidateRequestSender, int>();
        builder.AppendSenderWithResponse<CreateMemberSender, Abstractions.Models.Member, int>();
        builder.AppendSenderWithoutResponse<DeclineCandidateRequestSender, int>();
        builder.AppendSenderWithCollectionResponse<ReadAllCandidateRequestSender, int, CandidateRegisteredRequest>();
        builder.AppendSenderWithCollectionResponse<ReadAllMembersSender, int, Abstractions.Models.Member>();
        builder.AppendSenderWithResponse<ReadCandidateRequestSender, int, CandidateRegisteredRequest?>();
        builder.AppendSenderWithResponse<ReadMemberByDiscordIdSender, ulong, Abstractions.Models.Member?>();
        builder.AppendSenderWithResponse<ReadMemberByIdSender, int, Abstractions.Models.Member?>();
        builder.AppendSenderWithResponse<ReadMemberByNicknameSender, string, Abstractions.Models.Member?>();
        builder.AppendSenderWithResponse<ReadMemberBySteamIdSender, ulong, Abstractions.Models.Member?>();
        builder.AppendSenderWithResponse<RegisterCandidateRequestSender, Abstractions.Models.CandidateRequest, int>();
        builder.AppendSenderWithResponse<UpdateMemberSender, Abstractions.Models.Member, Abstractions.Models.Member>();

        builder.AppendSenderWithoutResponse<AcceptSquadJoinRequestSender, (short number, short part, int memberId)>();
        builder.AppendSenderWithResponse<CreateSquadSender, short, short>();
        builder.AppendSenderWithResponse<UpdateSquadSender, Abstractions.Models.Squad, Abstractions.Models.Squad>();
        builder.AppendSenderWithoutResponse<DeclineSquadJoinRequestSender, (short number, int memberId, bool byMember)>();
        builder.AppendSenderWithCollectionResponse<ReadAllSquadSender, int, ISquad>();
        builder.AppendSenderWithResponse<ReadSquadSender, int, Abstractions.Models.Squad>();
        builder.AppendSenderWithoutResponse<SendSquadJoinRequestSender, (int memberId, short squadNumber)>();
        builder.AppendSenderWithCollectionResponse<ReadSquadRequestSender, short, SquadRequest>();
        builder.AppendSenderWithCollectionResponse<ReadMemberSquadRequestSender, int, SquadRequest>();

        builder.AppendSenderWithoutResponse<OpenSquadMembershipHistorySender, (int memberId, short squadNumber)>();
        builder.AppendSenderWithoutResponse<CloseSquadMembershipHistorySender, int>();
        builder.AppendSenderWithCollectionResponse<ReadMemberSquadMembershipHistorySender, int, ISquadMembershipHistoryEntry>();
        builder.AppendSenderWithCollectionResponse<ReadSquadMembershipHistorySender, short, ISquadMembershipHistoryEntry>();

        return builder;
    }
}