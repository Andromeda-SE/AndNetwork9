using And9.Integration.VK.Listeners;
using And9.Lib.Broker;
using VkNet;

namespace And9.Integration.VK;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddSingleton<VkApi, VkBot>();
        services.AddHostedService(provider => (VkBot)provider.GetRequiredService(typeof(VkApi)));

        services.AddHostedService<WallPublish>();
        services.AddHostedService<ResolveVkUrl>();
    }
}