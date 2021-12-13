using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Server.Hubs;
using AndNetwork9.Server.Utility;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.AwardDispenser;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Backend.Senders.Elections;
using AndNetwork9.Shared.Backend.Senders.Steam;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Backend.Senders.VK;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndNetwork9.Server;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddInMemoryRateLimiting();
        services.Configure<ClientRateLimitOptions>(options =>
        {
            options.GeneralRules = new()
            {
                new()
                {
                    Endpoint = "get:/public/api/*",
                    Limit = 5,
                    PeriodTimespan = TimeSpan.FromMinutes(15),
                },
                new()
                {
                    Endpoint = "*:/api/*",
                    Limit = 120,
                    PeriodTimespan = TimeSpan.FromMinutes(1),
                },
                new()
                {
                    Endpoint = "*:/api/*",
                    Limit = 1500,
                    PeriodTimespan = TimeSpan.FromHours(1),
                },
                new()
                {
                    Endpoint = "*:/api/*",
                    Limit = 5000,
                    PeriodTimespan = TimeSpan.FromDays(1),
                },
                new()
                {
                    Endpoint = "*",
                    Limit = 6000,
                    PeriodTimespan = TimeSpan.FromDays(1),
                },
            };
        });
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IUserIdProvider, MemberIdProvider>();
        services.AddControllers(options =>
        {
            options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
            options.OutputFormatters.Add(new CustomSystemTextJsonOutputFormatter(new(JsonSerializerDefaults.Web)));
        });
        services.AddResponseCaching();
        services.AddRazorPages();

        services.AddSignalR();
        services.AddSwaggerGen();

        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] {"application/octet-stream"});
        });

        services.AddDbContext<ClanDataContext>(x =>
            x.UseNpgsql(Configuration["Postgres:ConnectionString"])
                .UseLazyLoadingProxies());

        RabbitConnectionPool.SetConfiguration(Configuration);
        services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddScoped<RepoGetFileSender>();
        services.AddScoped<SaveStaticFileSender>();
        services.AddScoped<PublishSender>();
        services.AddScoped<RepoSetFileSender>();
        services.AddScoped<NewRepoSender>();
        services.AddScoped<VoteSender>();
        services.AddScoped<SendSender>();
        services.AddScoped<GiveAwardSender>();
        services.AddScoped<UpdateUserSender>();
        services.AddScoped<ResolveDiscordUserNameSender>();
        services.AddScoped<ResolveVkUrlSender>();
        services.AddScoped<ResolveSteamUrlSender>();
        services.AddScoped<NewCandidateSender>();

        services.AddSingleton<IAuthorizationPolicyProvider, ClanPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, ClanPolicyProvider>();

        AuthExtensions.SetSalt(Configuration["STATIC_SALT"]);
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.Cookie.HttpOnly = false;
            options.LoginPath = string.Empty;
            options.AccessDeniedPath = string.Empty;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });
        services.AddAuthorization();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseClientRateLimiting();
        app.UseWebSockets();
        app.UseResponseCompression();

        app.UseSwagger();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "AndNetwork9"); });
        }

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseResponseCaching();

        app.UseCookiePolicy(new()
        {
            HttpOnly = HttpOnlyPolicy.None,
            MinimumSameSitePolicy = SameSiteMode.Unspecified,
        });
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapHub<ModelHub>("hub/model");
            endpoints.MapFallbackToFile("index.html");
        });
    }
}