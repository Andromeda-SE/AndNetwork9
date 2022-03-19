using And9.Lib.API;
using And9.Lib.Broker;
using And9.Service.Auth.Database;
using And9.Service.Auth.Listeners;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace And9.Service.Auth;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }
    public void ConfigureServices(IServiceCollection services)
    {
        Extensions.SetSalt(Configuration["STATIC_SALT"]);
        AuthOptions.IssuerKey = Configuration["ISSUER_KEY"];

        RabbitConnectionPool.SetConfiguration(Configuration);
        services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<AuthDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("and9.infra.redis"));

        services.AddHostedService<GeneratePasswordListener>();
        services.AddHostedService<LoginListener>();
        services.AddHostedService<SetPasswordListener>();
    }
}