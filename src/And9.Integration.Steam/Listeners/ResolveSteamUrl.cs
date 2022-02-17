using System.Runtime.CompilerServices;
using System.Web;
using And9.Integration.Steam.Models;
using And9.Integration.Steam.Senders;
using And9.Lib.Broker;
using RabbitMQ.Client;

namespace And9.Integration.Steam.Listeners;

public class ResolveSteamUrl : BaseRabbitListenerWithResponse<string, ulong?>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _steamKey;

    public ResolveSteamUrl(IConnection connection,
        IConfiguration configuration,
        ILogger<BaseRabbitListenerWithResponse<string, ulong?>> logger,
        IServiceScopeFactory serviceScopeFactory)
        : base(connection, ResolveSteamUrlSender.QUEUE_NAME, logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _steamKey = configuration["STEAM_KEY"];
    }

    protected override async Task<ulong?> GetResponseAsync(string request)
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        HttpClient httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

        if (string.IsNullOrWhiteSpace(request)) return null;
        request = request.Replace("http://", string.Empty);
        request = request.Replace("https://", string.Empty);
        request = request.Replace("www.", string.Empty);
        request = request.Replace("steamcommunity.com/id/", string.Empty);
        request = request.Replace("steamcommunity.com/profiles/", string.Empty);
        request = request.Trim('/');
        if (ulong.TryParse(request, out ulong result)) return result;
        PlayerActivityResult<ResolveSteamUrlResult>? answer =
            await httpClient.GetFromJsonAsync<PlayerActivityResult<ResolveSteamUrlResult>>(
                $"https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key={_steamKey}&vanityurl={HttpUtility.UrlEncode(request.Trim())}").ConfigureAwait(false);

        return answer?.Result?.SteamId;
    }
}