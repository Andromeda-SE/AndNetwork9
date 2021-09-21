﻿using System.Threading.Tasks;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.VK;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using VkNet;

namespace AndNetwork9.VK.Listeners
{
    public class WallPublish : BaseRabbitListenerWithoutResponse<string>
    {
        private readonly VkApi _vkApi;

        public WallPublish(IConnection connection, ILogger<BaseRabbitListenerWithoutResponse<string>> logger, VkApi vkApi) :
            base(connection, WallPublishSender.QUEUE_NAME, logger) => this._vkApi = vkApi;

        public override async Task Run(string request)
        {
            await _vkApi.Wall.PostAsync(new()
            {
                OwnerId = -207090758,
                FromGroup = true,
                Message = request,
                CloseComments = false,
            });
        }
    }
}