using And9.Lib.API;
using And9.Lib.Broker;
using And9.Service.Auth.Abstractions.Models;
using And9.Service.Auth.ConsumerStrategies;
using And9.Service.Auth.Database;
using And9.Service.Auth.Senders;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;
using Prometheus;
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

        services.AddDbContext<AuthDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("and9.infra.redis"));

        services.WithBroker(Configuration)
            .AppendConsumerWithResponse<LoginConsumer, AuthCredentials, string?>()
            .AppendConsumerWithoutResponse<SetPasswordConsumer, (int memberId, string newPassword)>()
            .AppendConsumerWithResponse<GeneratePasswordConsumer, int, string>()
            .AddAuthSenders()
            .AddCoreSenders()
            .Build();

        services.AddHealthChecks()
            .AddDbContextCheck<AuthDataContext>()
            .AddRedis("and9.infra.redis")
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