using System.Web;
using And9.Integration.Steam.Models;
using And9.Integration.Steam.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;

namespace And9.Integration.Steam.ConsumerStrategies;

[QueueName(ResolveSteamUrlSender.QUEUE_NAME)]
public class ResolveSteamUrlConsumerStrategy : IBrokerConsumerWithResponseStrategy<string, ulong?>
{
    private readonly HttpClient _httpClient;
    private readonly string _steamKey;

    public ResolveSteamUrlConsumerStrategy(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _steamKey = configuration["STEAM_KEY"];
        _httpClient = httpClientFactory.CreateClient("SteamApi");
    }

    public async ValueTask<ulong?> ExecuteAsync(string? request)
    {
        if (string.IsNullOrWhiteSpace(request)) return null;
        request = request.Replace("http://", string.Empty);
        request = request.Replace("https://", string.Empty);
        request = request.Replace("www.", string.Empty);
        request = request.Replace("steamcommunity.com/id/", string.Empty);
        request = request.Replace("steamcommunity.com/profiles/", string.Empty);
        request = request.Trim('/');
        if (ulong.TryParse(request, out ulong result)) return result;
        PlayerActivityResult<ResolveSteamUrlResult>? answer =
            await _httpClient.GetFromJsonAsync<PlayerActivityResult<ResolveSteamUrlResult>>(
                $"ISteamUser/ResolveVanityURL/v1/?key={_steamKey}&vanityurl={HttpUtility.UrlEncode(request.Trim())}").ConfigureAwait(false);

        return answer?.Result?.SteamId;
    }
}