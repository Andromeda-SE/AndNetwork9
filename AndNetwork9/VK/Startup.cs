using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.VK.Listeners;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VkNet;

namespace AndNetwork9.VK
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            RabbitConnectionPool.SetConfiguration(configuration);
            services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

            services.AddSingleton<VkApi, VkBot>();
            services.AddHostedService(provider => (VkBot)provider.GetService(typeof(VkApi))!);

            services.AddHostedService<WallPublish>();
            services.AddHostedService<ResolveVkUrl>();

        }
    }
}