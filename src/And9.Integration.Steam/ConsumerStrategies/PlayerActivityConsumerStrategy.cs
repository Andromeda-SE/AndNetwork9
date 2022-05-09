using And9.Integration.Steam.Models;
using And9.Integration.Steam.Senders;
using And9.Integration.Steam.Senders.Models;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;

namespace And9.Integration.Steam.ConsumerStrategies;

[QueueName(PlayerActivitySender.QUEUE_NAME)]
public class PlayerActivityConsumerStrategy : IBrokerConsumerWithResponseStrategy<ulong[], PlayerActivityResultNode[]>
{
    private readonly HttpClient _httpClient;
    private readonly string _steamKey;

    public PlayerActivityConsumerStrategy(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _steamKey = configuration["STEAM_KEY"];
        _httpClient = httpClientFactory.CreateClient("SteamApi");
    }

    public async ValueTask<PlayerActivityResultNode[]> ExecuteAsync(ulong[]? request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        PlayerActivityResultNode[][] results = new PlayerActivityResultNode[1 + (request.Length - 1) / 100][];
        for (int i = 0; i * 100 < request.Length; i++)
        {
            string ids = string.Join(',', request.Skip(i * 100).Take(100).Select(x => x.ToString("D")));
            // ReSharper disable once StringLiteralTypo
            PlayerActivityResult<PlayerActivityResultCollection>? result =
                await _httpClient.GetFromJsonAsync<PlayerActivityResult<PlayerActivityResultCollection>>(
                    $"ISteamUser/GetPlayerSummaries/v2/?key={_steamKey}&steamids={ids}").ConfigureAwait(false);
            results[i] = result?.Result?.Players ?? throw new();
        }

        return results.SelectMany(x => x).ToArray();
    }
}