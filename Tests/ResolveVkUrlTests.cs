using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AndNetwork9.VK.Listeners;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using VkNet;
using VkNet.Model;

namespace AndNetwork9.Tests
{
    public class ResolveVkUrlTests
    {
        private ResolveVkUrl _listener;
        private MethodInfo _method;

        public ResolveVkUrlTests()
        {
            VkApi? api = new VkApi(new NullLogger<VkApi>());
            api.AuthorizeAsync(new ApiAuthParams()
            {
                AccessToken = File.ReadAllText("Tokens/VkToken.txt")
            });
            _listener =
                new ResolveVkUrl(
                    new Mock<IConnection>().Object,
                    new NullLogger<ResolveVkUrl>(),
                    api
                );
            _method = _listener.GetType().GetMethod("GetResponseAsync", BindingFlags.Instance | BindingFlags.NonPublic);
            if (_method is null) throw new();
        }

        [Test]
        [TestCase("https://vk.com/moryakspb", ExpectedResult = 91777907L)]
        [TestCase("http://vk.com/moryakspb", ExpectedResult = 91777907L)]
        [TestCase("vk.com/moryakspb", ExpectedResult = 91777907L)]
        [TestCase("  https://vk.com/id91777907", ExpectedResult = 91777907L)]
        [TestCase("https://vk.com/id91777907", ExpectedResult = 91777907L)]
        [TestCase("http://vk.com/id91777907", ExpectedResult = 91777907L)]
        [TestCase("vk.com/id91777907", ExpectedResult = 91777907L)]
        [TestCase("moryakspb", ExpectedResult = 91777907L)]
        [TestCase("moryakspb  ", ExpectedResult = 91777907L)]
        [TestCase("id91777907", ExpectedResult = 91777907L)]
        [TestCase("91777907", ExpectedResult = 91777907L)]
        [TestCase("https://vk.com/moryakspb/", ExpectedResult = 91777907L)]
        [TestCase("http://vk.com/moryakspb/", ExpectedResult = 91777907L)]
        [TestCase("vk.com/moryakspb/", ExpectedResult = 91777907L)]
        [TestCase("https://vk.com/id91777907/", ExpectedResult = 91777907L)]
        [TestCase("http://vk.com/id91777907/", ExpectedResult = 91777907L)]
        [TestCase("vk.com/id91777907/", ExpectedResult = 91777907L)]
        [TestCase("moryakspb/", ExpectedResult = 91777907L)]
        [TestCase("id91777907/", ExpectedResult = 91777907L)]
        [TestCase("91777907/", ExpectedResult = 91777907L)]
        [TestCase("durov", ExpectedResult = 1L)]
        [TestCase("fafdqwfgqf###%%\\^%^", ExpectedResult = null)]
        [TestCase("", ExpectedResult = null)]
        [TestCase("http://localhost:15672/#/queues/%2F/test", ExpectedResult = null)]
        public async Task<long?> Test1(string value)
        {
            long? result = await (_method.Invoke(_listener, new object?[] {value}) as Task<long?>);
            return result;
        }
    }
}