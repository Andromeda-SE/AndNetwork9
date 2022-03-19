using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Service.Core.Senders;
using And9.Service.Election.Database;
using And9.Service.Election.Listeners;
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
        RabbitConnectionPool.SetConfiguration(Configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<ElectionDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));

        services.AddSingleton<MemberCrudSender>();
        services.AddSingleton<SendLogMessageSender>();

        services.AddTransient<NewElectionStrategy>();
        services.AddTransient<StartAnnouncementStrategy>();
        services.AddTransient<StartVoteStrategy>();

        services.AddHostedService<ElectionWatcher>();

        services.AddHostedService<CurrentElectionListener>();
        services.AddHostedService<RegisterListener>();
        services.AddHostedService<CancelRegisterListener>();
        services.AddHostedService<VoteListener>();

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