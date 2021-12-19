using System.Threading.Tasks;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.VK;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using VkNet;
using VkNet.Enums;
using VkNet.Model;

namespace AndNetwork9.VK.Listeners;

public class ResolveVkUrl : BaseRabbitListenerWithResponse<string, long?>
{
    private readonly VkApi _vkApi;

    public ResolveVkUrl(IConnection connection, ILogger<ResolveVkUrl> logger, VkApi vkApi) :
        base(connection, ResolveVkUrlSender.QUEUE_NAME, logger) => _vkApi = vkApi;

    protected override async Task<long?> GetResponseAsync(string request)
    {
        request = request.Replace("http://", string.Empty);
        request = request.Replace("https://", string.Empty);
        request = request.Replace("www.", string.Empty);
        request = request.Replace("vk.com/id", string.Empty);
        request = request.Replace("vk.com/", string.Empty);
        request = request.Trim().Trim('/');
        if (string.IsNullOrWhiteSpace(request)) return null;
        if (long.TryParse(request, out long result)) return result;
        VkObject? answer = await _vkApi.Utils.ResolveScreenNameAsync(request);
        return answer is not null && answer.Type == VkObjectType.User ? answer.Id : null;
    }
}