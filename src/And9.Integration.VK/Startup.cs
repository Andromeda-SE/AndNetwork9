using And9.Integration.VK.ConsumerStrategies;
using And9.Integration.VK.HealthChecks;
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
        services.AddSingleton<VkApi, VkBot>();
        services.AddHostedService(provider => (VkBot)provider.GetRequiredService(typeof(VkApi)));

        services.WithBroker(Configuration)
            .AppendConsumerWithoutResponse<WallPublishConsumerStrategy, string>()
            .AppendConsumerWithResponse<ResolveVkUrlConsumerStrategy, string, long?>()
            .Build();

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