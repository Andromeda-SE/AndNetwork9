using And9.Integration.Discord.Database;
using And9.Integration.Discord.HealthChecks;
using And9.Integration.Discord.Listeners;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
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
        RabbitConnectionPool.SetConfiguration(Configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<DiscordDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));

        services.AddHealthChecks()
            .AddDbContextCheck<DiscordDataContext>()
            .AddRabbitMQ()
            .AddCheck<DiscordConnectionHealthCheck>("DiscordConnection")
            .ForwardToPrometheus();

        services.AddSingleton<DiscordBot>();
        services.AddHostedService(provider => (DiscordBot)provider.GetService(typeof(DiscordBot))!);

        services.AddSingleton<MemberCrudSender>();
        services.AddSingleton<SyncChannelsSender>();
        services.AddSingleton<SyncUserSender>();
        services.AddSingleton<SyncRolesSender>();

        services.AddHostedService<SendDirectMessageListener>();
        services.AddHostedService<SyncUserListener>();
        services.AddHostedService<SyncRolesListener>();
        services.AddHostedService<SyncChannelsListener>();
        services.AddHostedService<SendLogMessageListener>();
        services.AddHostedService<SendCandidateRequestListener>();
        services.AddHostedService<ResolveDiscordUserNameListener>();
        services.AddHostedService<RegisterChannelListener>();
        services.AddHostedService<RegisterChannelCategoryListener>();
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