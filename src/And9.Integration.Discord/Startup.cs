using And9.Integration.Discord.Abstractions.Interfaces;
using And9.Integration.Discord.Abstractions.Models;
using And9.Integration.Discord.ConsumerStrategies;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.HealthChecks;
using And9.Integration.Discord.Senders;
using And9.Integration.Discord.Senders.Models;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace And9.Integration.Discord;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<DiscordDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));

        services.WithBroker(Configuration)
            .AppendConsumerWithoutResponse<SendDirectMessageConsumerStrategy, SendDirectMessageArg>()
            .AppendConsumerWithoutResponse<SyncUserConsumerStrategy, Member>()
            .AppendConsumerWithoutResponse<SyncRolesConsumerStrategy, object>()
            .AppendConsumerWithoutResponse<SyncChannelsConsumerStrategy, object>()
            .AppendConsumerWithoutResponse<SendLogMessageConsumerStrategy, string>()
            .AppendConsumerWithoutResponse<SendCandidateRequestConsumerStrategy, CandidateRequest>()
            .AppendConsumerWithResponse<ResolveDiscordUserNameConsumerStrategy, string, ulong?>()
            .AppendConsumerWithResponse<RegisterChannelConsumerStrategy, Channel, bool>()
            .AppendConsumerWithResponse<RegisterChannelCategoryConsumerStrategy, ulong, bool>()
            .AppendConsumerWithResponse<CreateChannelConsumerStrategy, IChannel, IChannel>()
            .AppendConsumerWithResponse<UpdateChannelConsumerStrategy, IChannel, IChannel>()
            .AddCoreSenders()
            .AddDiscordSenders()
            .Build();

        services.AddSingleton<DiscordBot>();
        services.AddHostedService(provider => (DiscordBot)provider.GetService(typeof(DiscordBot))!);

        services.AddHealthChecks()
            .AddDbContextCheck<DiscordDataContext>()
            .AddRabbitMQ()
            .AddCheck<DiscordConnectionHealthCheck>("DiscordConnection")
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