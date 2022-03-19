using And9.Integration.Steam.HealthChecks;
using And9.Integration.Steam.Listeners;
using And9.Lib.Broker;
using Prometheus;

namespace And9.Integration.Steam;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }
    public void ConfigureServices(IServiceCollection services)
    {
        RabbitConnectionPool.SetConfiguration(Configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddScoped<HttpClient>();

        services.AddHostedService<PlayerActivity>();
        services.AddHostedService<ResolveSteamUrl>();
        services.AddHealthChecks()
            .AddRabbitMQ()
            .AddCheck<SteamHealthCheck>("SteamConnection")
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