using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Steam.Listeners;
using AndNetwork9.VK.Listeners;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using VkNet;
using VkNet.Model;

namespace AndNetwork9.Tests
{
    public class ResolveSteamUrlTests
    {
        private ResolveSteamUrl _listener;
        private MethodInfo _method;

        public ResolveSteamUrlTests()
        {
            _listener = new ResolveSteamUrl(new Mock<IConnection>().Object, new Config()
            {
                Parameters = new Dictionary<string, string>()
                {
                    {"STEAM_KEY", File.ReadAllText("Tokens/SteamToken.txt")}
                }
            }, new NullLogger<BaseRabbitListenerWithResponse<string, ulong?>>(), new HttpClient());
            _method = _listener.GetType().GetMethod("GetResponseAsync", BindingFlags.Instance | BindingFlags.NonPublic);
            if (_method is null) throw new();
        }

        [Test]
        [TestCase("https://steamcommunity.com/id/moryakspb", ExpectedResult = 76561198109866439UL)]
        [TestCase("http://steamcommunity.com/id/moryakspb", ExpectedResult = 76561198109866439UL)]
        [TestCase("steamcommunity.com/id/moryakspb", ExpectedResult = 76561198109866439UL)]
        [TestCase("  https://steamcommunity.com/profiles/76561198109866439", ExpectedResult = 76561198109866439UL)]
        [TestCase("https://steamcommunity.com/profiles/76561198109866439", ExpectedResult = 76561198109866439UL)]
        [TestCase("http://steamcommunity.com/profiles/76561198109866439", ExpectedResult = 76561198109866439UL)]
        [TestCase("steamcommunity.com/profiles/76561198109866439", ExpectedResult = 76561198109866439UL)]
        [TestCase("moryakspb", ExpectedResult = 76561198109866439UL)]
        [TestCase("moryakspb  ", ExpectedResult = 76561198109866439UL)]
        [TestCase("76561198109866439", ExpectedResult = 76561198109866439UL)]
        [TestCase("https://steamcommunity.com/id/moryakspb/", ExpectedResult = 76561198109866439UL)]
        [TestCase("http://steamcommunity.com/id/moryakspb/", ExpectedResult = 76561198109866439UL)]
        [TestCase("steamcommunity.com/id/moryakspb/", ExpectedResult = 76561198109866439UL)]
        [TestCase("https://steamcommunity.com/profiles/76561198109866439/", ExpectedResult = 76561198109866439UL)]
        [TestCase("http://steamcommunity.com/profiles/76561198109866439/", ExpectedResult = 76561198109866439UL)]
        [TestCase("steamcommunity.com/profiles/76561198109866439/", ExpectedResult = 76561198109866439UL)]
        [TestCase("moryakspb/", ExpectedResult = 76561198109866439UL)]
        [TestCase("76561198109866439/", ExpectedResult = 76561198109866439UL)]
        [TestCase("fafdqwfgqf###%%\\^%^", ExpectedResult = null)]
        [TestCase("", ExpectedResult = null)]
        [TestCase("http://localhost:15672/#/queues/%2F/test", ExpectedResult = null)]
        public async Task<ulong?> Test1(string value)
        {
            ulong? result = await (_method.Invoke(_listener, new object?[] { value }) as Task<ulong?>);
            return result;
        }
    }
}