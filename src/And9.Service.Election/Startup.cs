using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Senders;
using And9.Service.Election.ConsumerStrategies;
using And9.Service.Election.Database;
using And9.Service.Election.Services.ElectionWatcher;
using And9.Service.Election.Services.ElectionWatcher.Strategies;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace And9.Service.Election;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ElectionDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));

        services.WithBroker(Configuration)
            .AppendConsumerWithResponse<VoteConsumerStrategy, (int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes), bool>()
            .AppendConsumerWithResponse<RegisterConsumerStrategy, (int memberId, Direction direction), bool>()
            .AppendConsumerWithCollectionResponse<CurrentElectionConsumerStrategy, int, Abstractions.Models.Election>()
            .AppendConsumerWithResponse<CancelRegisterConsumerStrategy, int, bool>()
            .AddGatewaySenders()
            .AddCoreSenders()
            .Build();

        services.AddScoped<NewElectionStrategy>();
        services.AddScoped<StartAnnouncementStrategy>();
        services.AddScoped<StartVoteStrategy>();

        services.AddHostedService<ElectionWatcher>();

        services.AddHealthChecks()
            .AddDbContextCheck<ElectionDataContext>()
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