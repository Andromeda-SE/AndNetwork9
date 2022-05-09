using And9.Integration.VK.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using VkNet;

namespace And9.Integration.VK.ConsumerStrategies;

[QueueName(WallPublishSender.QUEUE_NAME)]
public class WallPublishConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<string>
{
    private readonly long _groupId;
    private readonly VkApi _vkApi;

    public WallPublishConsumerStrategy(VkApi vkApi, IConfiguration configuration)
    {
        _vkApi = vkApi;
        _groupId = -long.Parse(configuration["Vk_GroupId"]);
        if (_groupId > 0) throw new ArgumentException();
    }

    public async ValueTask ExecuteAsync(string arg)
    {
        await _vkApi.Wall.PostAsync(new()
        {
            OwnerId = _groupId,
            FromGroup = true,
            Message = arg,
            CloseComments = false,
        }).ConfigureAwait(false);
    }
}