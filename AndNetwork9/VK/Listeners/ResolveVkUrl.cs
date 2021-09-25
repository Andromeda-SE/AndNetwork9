using System;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.VK;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using VkNet;
using VkNet.Enums;


namespace AndNetwork9.VK.Listeners
{
    public class ResolveVkUrl : BaseRabbitListenerWithResponse<string, long?>
    {
        private readonly long _groupId;
        private readonly VkApi _vkApi;
       

        public ResolveVkUrl(IConnection connection, ILogger<ResolveVkUrl> logger,
            IConfiguration configuration, VkApi vkApi) :
            base(connection, ResolveVkUrlSender.QUEUE_NAME, logger)
        {
            _vkApi = vkApi;
            _groupId = -long.Parse(configuration["Vk_GroupId"]);
            if (_groupId > 0) throw new ArgumentException();
        }
        protected override async Task<long?> GetResponseAsync(string request)
        {
            request = request.Replace("http://", string.Empty);
            request = request.Replace("https://", string.Empty);
            request = request.Replace("vk.com/", string.Empty);
            request = request.Replace("vk.com/id", string.Empty);
            request = request.Trim('/');
            if (long.TryParse(request, out long result)) return result;
            var answer = await _vkApi.Utils.ResolveScreenNameAsync(_groupId.ToString());
            return answer.Type==VkObjectType.User ? answer.Id:null;
        }
    }
}