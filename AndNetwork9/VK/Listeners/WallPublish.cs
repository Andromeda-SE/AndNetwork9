using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.VK;
using AndNetwork9.VK;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using VkNet;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

    public class WallPublish : BaseRabbitListenerWithoutResponse<string>
{
    public override Task Run(string request)
    {

        var post = vkapi.Wall.Post(new WallPostParams
        {
            OwnerId = -207090758,
            FromGroup = true,
            Message = request,
            CloseComments = false,


        });
        return Task.CompletedTask;
    }

    public WallPublish(IConnection connection,  ILogger<BaseRabbitListenerWithoutResponse<string>> logger, VkApi vkapi) : base(connection, WallPublishSender.QUEUE_NAME, logger)
    {
        this.vkapi = vkapi;
    }

    private readonly VkApi vkapi;
}