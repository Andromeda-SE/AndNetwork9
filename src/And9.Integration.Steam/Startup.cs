using And9.Integration.Steam.ConsumerStrategies;
using And9.Integration.Steam.HealthChecks;
using And9.Integration.Steam.Senders.Models;
using And9.Lib.Broker;
using Prometheus;

namespace And9.Integration.Steam;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient("SteamApi", client => client.BaseAddress = new("https://api.steampowered.com/"));

        services.WithBroker(Configuration)
            .AppendConsumerWithResponse<PlayerActivityConsumerStrategy, ulong[], PlayerActivityResultNode[]>()
            .AppendConsumerWithResponse<ResolveSteamUrlConsumerStrategy, string, ulong?>()
            .Build();

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