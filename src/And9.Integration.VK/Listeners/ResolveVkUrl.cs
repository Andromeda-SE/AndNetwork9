using And9.Integration.VK.Senders;
using And9.Lib.Broker;
using RabbitMQ.Client;
using VkNet;
using VkNet.Enums;
using VkNet.Model;

namespace And9.Integration.VK.Listeners;

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
        VkObject? answer = await _vkApi.Utils.ResolveScreenNameAsync(request).ConfigureAwait(false);
        return answer is not null && answer.Type == VkObjectType.User ? answer.Id : null;
    }
}