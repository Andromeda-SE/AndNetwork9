using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using VkNet;
using VkNet.Model;

namespace AndNetwork9.VK;

public class VkBot : VkApi, IHostedService
{
    private readonly string _authorizationToken;

    public VkBot(IConfiguration configuration)
    {
        _authorizationToken = configuration["Vk_Token"];
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await AuthorizeAsync(new ApiAuthParams
        {
            AccessToken = _authorizationToken,
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await LogOutAsync();
    }
}