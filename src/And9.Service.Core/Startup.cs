using And9.Lib.Broker;
using And9.Service.Core.Database;
using And9.Service.Core.Listeners;
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
        RabbitConnectionPool.SetConfiguration(Configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<CoreDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));

        services.AddScoped<MemberCrudSender>();

        services.AddHostedService<AcceptCandidateListener>();
        services.AddHostedService<DeclineCandidateListener>();
        services.AddHostedService<RegisterCandidateRequestListener>();
        services.AddHostedService<MemberCrudListener>();
        services.AddHostedService<CandidateRequestListener>();
        services.AddHostedService<ReadMemberBySteamIdListener>();
        services.AddHostedService<ReadMemberByDiscordIdListener>();
        services.AddHostedService<ReadMemberByNicknameListener>();

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