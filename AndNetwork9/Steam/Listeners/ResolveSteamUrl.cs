using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Steam;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Steam.Listeners
{
    public class ResolveSteamUrl : BaseRabbitListenerWithResponse<string, ulong?>
    {
        private readonly HttpClient _httpClient;
        private readonly string _steamKey;

        public ResolveSteamUrl(IConnection connection, IConfiguration configuration,
            ILogger<BaseRabbitListenerWithResponse<string, ulong?>> logger, HttpClient httpClient)
            : base(connection, ResolveSteamUrlSender.QUEUE_NAME, logger)
        {
            _httpClient = httpClient;
            _steamKey = configuration["STEAM_KEY"];
        }

        protected override async Task<ulong?> GetResponseAsync(string request)
        {
            request = request.Replace("http://", string.Empty);
            request = request.Replace("https://", string.Empty);
            request = request.Replace("steamcommunity.com/id/", string.Empty);
            request = request.Replace("steamcommunity.com/profiles/", string.Empty);
            request = request.Trim('/');
            if (ulong.TryParse(request, out ulong result)) return result;
            PlayerActivityResult<ResolveSteamUrlResult>? answer =
                await _httpClient.GetFromJsonAsync<PlayerActivityResult<ResolveSteamUrlResult>>(
                    $"https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key={_steamKey}&vanityurl={HttpUtility.UrlEncode(request)}");
            return answer?.Result.SteamId;
        }
    }
}