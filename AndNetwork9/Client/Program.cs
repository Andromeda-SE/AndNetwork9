using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using AndNetwork9.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AndNetwork9.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new(builder.HostEnvironment.BaseAddress),
            DefaultRequestVersion = new(3, 0),
            Timeout = TimeSpan.FromSeconds(30),
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher,
        });

        builder.Services.AddSingleton<ModelService>();

        builder.Services.AddScoped<AuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();

        CultureInfo.DefaultThreadCurrentCulture = new("ru");
        CultureInfo.DefaultThreadCurrentUICulture = new("ru");

        await builder.Build().RunAsync();
    }
}