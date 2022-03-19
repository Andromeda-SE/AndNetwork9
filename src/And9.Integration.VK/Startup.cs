using And9.Integration.VK.HealthChecks;
using And9.Integration.VK.Listeners;
using And9.Lib.Broker;
using Prometheus;
using VkNet;

namespace And9.Integration.VK;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }
    public void ConfigureServices(IServiceCollection services)
    {
        RabbitConnectionPool.SetConfiguration(Configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddSingleton<VkApi, VkBot>();
        services.AddHostedService(provider => (VkBot)provider.GetRequiredService(typeof(VkApi)));

        services.AddHostedService<WallPublish>();
        services.AddHostedService<ResolveVkUrl>();

        services.AddHealthChecks()
            .AddRabbitMQ()
            .AddCheck<VkHealthCheck>("VkConnection")
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