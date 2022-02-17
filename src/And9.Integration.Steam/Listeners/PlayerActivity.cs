using System.Runtime.CompilerServices;
using And9.Integration.Steam.Models;
using And9.Integration.Steam.Senders;
using And9.Integration.Steam.Senders.Models;
using And9.Lib.Broker;
using RabbitMQ.Client;

namespace And9.Integration.Steam.Listeners;

public class PlayerActivity : BaseRabbitListenerWithResponse<ulong[], PlayerActivityResultNode[]>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _steamKey;

    public PlayerActivity(
        IConnection connection,
        IConfiguration configuration,
        ILogger<BaseRabbitListenerWithResponse<ulong[], PlayerActivityResultNode[]>> logger,
        IServiceScopeFactory serviceScopeFactory)
        : base(connection, PlayerActivitySender.QUEUE_NAME, logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _steamKey = configuration["STEAM_KEY"];
    }

    protected override async Task<PlayerActivityResultNode[]> GetResponseAsync(ulong[] request)
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        HttpClient httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

        PlayerActivityResultNode[][] results = new PlayerActivityResultNode[1 + (request.Length - 1) / 100][];
        for (int i = 0; i * 100 < request.Length; i++)
        {
            string ids = string.Join(',', request.Skip(i * 100).Take(100).Select(x => x.ToString("D")));
            // ReSharper disable once StringLiteralTypo
            PlayerActivityResult<PlayerActivityResultCollection>? result =
                await httpClient.GetFromJsonAsync<PlayerActivityResult<PlayerActivityResultCollection>>(
                    $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_steamKey}&steamids={ids}").ConfigureAwait(false);
            results[i] = result?.Result?.Players ?? throw new();
        }

        return results.SelectMany(x => x).ToArray();
    }
}