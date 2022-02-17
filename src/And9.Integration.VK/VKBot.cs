using VkNet;
using VkNet.Model;

namespace And9.Integration.VK;

public class VkBot : VkApi, IHostedService
{
    private readonly string _authorizationToken;

    public VkBot(IConfiguration configuration) => _authorizationToken = configuration["Vk_Token"];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await AuthorizeAsync(new ApiAuthParams
        {
            AccessToken = _authorizationToken,
        }).ConfigureAwait(false);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await LogOutAsync().ConfigureAwait(false);
    }
}