using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Steam;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Steam.Listeners;

public class PlayerActivity : BaseRabbitListenerWithResponse<ulong[], PlayerActivityResultNode[]>
{
    private readonly HttpClient _httpClient;
    private readonly string _steamKey;

    public PlayerActivity(IConnection connection, IConfiguration configuration,
        ILogger<BaseRabbitListenerWithResponse<ulong[], PlayerActivityResultNode[]>> logger, HttpClient httpClient)
        : base(connection, PlayerActivitySender.QUEUE_NAME, logger)
    {
        _httpClient = httpClient;
        _steamKey = configuration["STEAM_KEY"];
    }

    protected override async Task<PlayerActivityResultNode[]> GetResponseAsync(ulong[] request)
    {
        PlayerActivityResult<PlayerActivityResultCollection>? result =
            await _httpClient.GetFromJsonAsync<PlayerActivityResult<PlayerActivityResultCollection>>(
                $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_steamKey}&steamids={string.Join(',', request.Take(100).Select(x => x.ToString("D")))}");
        return result?.Result.Players ?? throw new();
    }
}