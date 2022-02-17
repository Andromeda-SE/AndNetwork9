using And9.Lib.API;
using And9.Lib.Broker;
using And9.Service.Auth.Database;
using And9.Service.Auth.Listeners;
using And9.Service.Core.Database;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace And9.Service.Auth;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        Extensions.SetSalt(configuration["STATIC_SALT"]);
        AuthOptions.IssuerKey = configuration["ISSUER_KEY"];

        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<AuthDataContext>(x => x.UseNpgsql(configuration["Postgres:ConnectionString"]));
        services.AddDbContext<CoreDataContext>(x => x.UseNpgsql(configuration["Postgres:ConnectionString"]));
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("and9.infra.redis"));

        services.AddHostedService<GeneratePasswordListener>();
        services.AddHostedService<LoginListener>();
        services.AddHostedService<SetPasswordListener>();
    }
}