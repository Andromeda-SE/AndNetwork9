using And9.Integration.VK.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using VkNet;
using VkNet.Enums;
using VkNet.Model;

namespace And9.Integration.VK.ConsumerStrategies;

[QueueName(ResolveVkUrlSender.QUEUE_NAME)]
public class ResolveVkUrlConsumerStrategy : IBrokerConsumerWithResponseStrategy<string, long?>
{
    private readonly VkApi _vkApi;

    public ResolveVkUrlConsumerStrategy(VkApi vkApi) => _vkApi = vkApi;


    public async ValueTask<long?> ExecuteAsync(string? request)
    {
        if (request is null) return null;
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