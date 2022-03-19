using And9.Gateway.Clan.Senders;
using And9.Integration.Steam.Senders;
using And9.Lib.Broker;
using And9.Service.Award.Database;
using And9.Service.Award.Listeners;
using And9.Service.Award.Senders;
using And9.Service.Award.Services;
using And9.Service.Award.Services.AwardDispenser;
using And9.Service.Award.Services.AwardDispenser.Strategy;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace And9.Service.Award;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }
    public void ConfigureServices(IServiceCollection services)
    {
        RabbitConnectionPool.SetConfiguration(Configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<AwardDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));

        services.AddScoped<MemberCrudSender>();
        services.AddScoped<AwardCrudSender>();
        services.AddScoped<RaiseMemberUpdateSender>();
        services.AddScoped<PlayerActivitySender>();

        services.AddSingleton<InGameAwardDispenserStrategy>();
        services.AddSingleton<TogetherInGameAwardDispenserStrategy>();

        services.AddHealthChecks()
            .AddDbContextCheck<AwardDataContext>()
            .AddRabbitMQ()
            .ForwardToPrometheus();

        services.AddHostedService<RankUpdaterService>();
        services.AddHostedService<AwardDispenserService<InGameAwardDispenserStrategy>>();
        services.AddHostedService<AwardDispenserService<TogetherInGameAwardDispenserStrategy>>();

        services.AddHostedService<AwardModelCrudListener>();
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