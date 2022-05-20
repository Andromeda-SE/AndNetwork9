using And9.Gateway.Clan.Client.Connections;
using And9.Gateway.Clan.Client.Interfaces;
using And9.Service.Award.Abstractions.Models;
using And9.Service.Core.Abstractions.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace And9.Gateway.Clan.Client;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        AuthConnection? connection = null;
        ServiceProvider localProvider = services.BuildServiceProvider();
        connection = new(GetDefaultBuilder().WithUrl(Path.Combine(GetBaseAddress(localProvider), "auth"),
            options => { options.AccessTokenProvider = (connection?.GetToken() ?? ValueTask.FromResult(string.Empty)).AsTask!; }).Build());
        localProvider.Dispose();
        services.AddSingleton<AuthConnection>(_ => connection);
        services.AddSingleton<IAuthTokenProvider>(provider => provider.GetRequiredService<AuthConnection>());

        services.AddSingleton<CoreConnection>(
            provider => new(
                GetDefaultBuilder().WithUrl(Path.Combine(GetBaseAddress(provider), "core"),
                    options => { options.AccessTokenProvider = GetTokenTask(provider); }).Build(),
                provider.GetRequiredService<IAuthTokenProvider>(),
                provider.GetRequiredService<ILogger<CoreConnection>>()));
        services.AddHostedService(provider => provider.GetRequiredService<CoreConnection>());
        services.AddSingleton<DiscordConnection>(
            provider => new(
                GetDefaultBuilder().WithUrl(Path.Combine(GetBaseAddress(provider), "discord"),
                    options => { options.AccessTokenProvider = GetTokenTask(provider); }).Build(),
                provider.GetRequiredService<IAuthTokenProvider>(),
                provider.GetRequiredService<ILogger<DiscordConnection>>()));
        services.AddHostedService(provider => provider.GetRequiredService<DiscordConnection>());
        services.AddSingleton<ElectionConnection>(
            provider => new(
                GetDefaultBuilder().WithUrl(Path.Combine(GetBaseAddress(provider), "election"),
                    options => { options.AccessTokenProvider = GetTokenTask(provider); }).Build(),
                provider.GetRequiredService<IAuthTokenProvider>(),
                provider.GetRequiredService<ILogger<ElectionConnection>>()));
        services.AddHostedService(provider => provider.GetRequiredService<ElectionConnection>());
        services.AddSingleton<RepositoryConnection<Member>>(
            provider => new(
                GetDefaultBuilder().WithUrl(Path.Combine(GetBaseAddress(provider), "model/member"),
                    options => { options.AccessTokenProvider = GetTokenTask(provider); }).Build(),
                provider.GetRequiredService<IAuthTokenProvider>(),
                provider.GetRequiredService<ILogger<RepositoryConnection<Member>>>()));
        services.AddHostedService(provider => provider.GetRequiredService<RepositoryConnection<Member>>());
        services.AddSingleton<RepositoryConnection<Award>>(
            provider => new(
                GetDefaultBuilder().WithUrl(Path.Combine(GetBaseAddress(provider), "model/award"),
                    options => { options.AccessTokenProvider = GetTokenTask(provider); }).Build(),
                provider.GetRequiredService<IAuthTokenProvider>(),
                provider.GetRequiredService<ILogger<RepositoryConnection<Award>>>()));
        services.AddHostedService(provider => provider.GetRequiredService<RepositoryConnection<Award>>());
        services.AddSingleton<ReadOnlyRepositoryConnection<Squad>>(
            provider => new(
                GetDefaultBuilder().WithUrl(Path.Combine(GetBaseAddress(provider), "model/squad"),
                    options => { options.AccessTokenProvider = GetTokenTask(provider); }).Build(),
                provider.GetRequiredService<IAuthTokenProvider>(),
                provider.GetRequiredService<ILogger<ReadOnlyRepositoryConnection<Squad>>>()));
        services.AddHostedService(provider => provider.GetRequiredService<ReadOnlyRepositoryConnection<Squad>>());
    }

    private static IHubConnectionBuilder GetDefaultBuilder() => new HubConnectionBuilder().WithAutomaticReconnect() /*.AddMessagePackProtocol()*/;
    private static string GetBaseAddress(IServiceProvider provider) => $"ws://{provider.GetRequiredService<IConfiguration>()["CLAN_DOMAIN"]}/hub";
    private static Func<Task<string?>> GetTokenTask(IServiceProvider provider) => provider.GetRequiredService<IAuthTokenProvider>().GetToken().AsTask;
}