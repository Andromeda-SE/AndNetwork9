using And9.Integration.VK.Senders;
using And9.Lib.Broker;
using RabbitMQ.Client;
using VkNet;

namespace And9.Integration.VK.Listeners;

public class WallPublish : BaseRabbitListenerWithoutResponse<string>
{
    private readonly long _groupId;
    private readonly VkApi _vkApi;

    public WallPublish(IConnection connection, ILogger<BaseRabbitListenerWithoutResponse<string>> logger,
        IConfiguration configuration, VkApi vkApi) :
        base(connection, WallPublishSender.QUEUE_NAME, logger)
    {
        _vkApi = vkApi;
        _groupId = -long.Parse(configuration["Vk_GroupId"]);
        if (_groupId > 0) throw new ArgumentException();
    }

    public override async Task Run(string request)
    {
        await _vkApi.Wall.PostAsync(new()
        {
            OwnerId = _groupId,
            FromGroup = true,
            Message = request,
            CloseComments = false,
        }).ConfigureAwait(false);
    }
}