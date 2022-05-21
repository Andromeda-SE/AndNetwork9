using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.ConsumerStrategy.CandidateRequests;
using And9.Service.Core.ConsumerStrategy.Member;
using And9.Service.Core.ConsumerStrategy.Specializations;
using And9.Service.Core.ConsumerStrategy.Squad;
using And9.Service.Core.ConsumerStrategy.Squad.SquadMembershipHistory;
using And9.Service.Core.ConsumerStrategy.Squad.SquadRequest;
using And9.Service.Core.Database;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace And9.Service.Core;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<CoreDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));

        services.WithBroker(Configuration)
            .AppendConsumerWithoutResponse<AcceptCandidateRequestConsumerStrategy, int>()
            .AppendConsumerWithResponse<CreateMemberConsumerStrategy, Member, int>()
            .AppendConsumerWithoutResponse<DeclineCandidateRequestConsumerStrategy, int>()
            .AppendConsumerWithCollectionResponse<ReadAllCandidateRequestConsumerStrategy, int, CandidateRegisteredRequest>()
            .AppendConsumerWithCollectionResponse<ReadAllMembersConsumerStrategy, int, Member>()
            .AppendConsumerWithResponse<ReadCandidateRequestConsumerStrategy, int, CandidateRegisteredRequest?>()
            .AppendConsumerWithResponse<ReadMemberByDiscordIdConsumerStrategy, ulong, Member?>()
            .AppendConsumerWithResponse<ReadMemberByIdConsumerStrategy, int, Member?>()
            .AppendConsumerWithResponse<ReadMemberByNicknameConsumerStrategy, string, Member?>()
            .AppendConsumerWithResponse<ReadMemberBySteamIdConsumerStrategy, ulong, Member?>()
            .AppendConsumerWithResponse<RegisterCandidateRequestConsumerStrategy, CandidateRequest, int>()
            .AppendConsumerWithResponse<UpdateMemberConsumerStrategy, Member, Member>()
            .AppendConsumerWithResponse<ReadSquadConsumerStrategy, int, Squad?>()
            .AppendConsumerWithCollectionResponse<ReadAllSquadConsumerStrategy, int, ISquad>()
            .AppendConsumerWithResponse<UpdateSquadConsumerStrategy, Squad, Squad>()
            .AppendConsumerWithResponse<CreateSquadConsumerStrategy, short, short>()
            .AppendConsumerWithoutResponse<AcceptSquadJoinRequestConsumerStrategy, (short number, short squadPart, int memberId)>()
            .AppendConsumerWithoutResponse<DeclineSquadJoinRequestConsumerStrategy, (short number, int memberId, bool byMember)>()
            .AppendConsumerWithCollectionResponse<ReadMemberSquadRequestConsumerStrategy, int, SquadRequest>()
            .AppendConsumerWithoutResponse<SendSquadJoinRequestConsumerStrategy, (int memberId, short squadNumber)>()
            .AppendConsumerWithCollectionResponse<ReadSquadRequestConsumerStrategy, short, SquadRequest>()
            .AppendConsumerWithCollectionResponse<ReadMemberSquadMembershipHistoryConsumerStrategy, int, ISquadMembershipHistoryEntry>()
            .AppendConsumerWithCollectionResponse<ReadSquadMembershipHistoryConsumerStrategy, short, ISquadMembershipHistoryEntry>()
            .AppendConsumerWithoutResponse<OpenSquadMembershipHistoryConsumerStrategy, (int memberId, short squadNumber)>()
            .AppendConsumerWithoutResponse<CloseSquadMembershipHistoryConsumerStrategy, int>()
            .AppendConsumerWithoutResponse<WithdrawSpecializationConsumerStrategy, (int memberId, int specialzationId, int callerId)>()
            .AppendConsumerWithoutResponse<ApproveSpecializationConsumerStrategy, (int memberId, int specialzationId, int callerId)>()
            .AppendConsumerWithCollectionResponse<ReadAllSpecializationsConsumerStrategy, int, Specialization>()
            .AddCoreSenders()
            .AddGatewaySenders()
            .Build();

        services.AddHealthChecks()
            .AddDbContextCheck<CoreDataContext>()
            .AddRabbitMQ()
            .ForwardToPrometheus();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseRouting();
        app.UseHttpMetrics();
        app.UseMetricServer();
        app.UseHealthChecks("/health");
    }
}