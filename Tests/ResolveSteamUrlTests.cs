using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Steam.Listeners;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;

namespace AndNetwork9.Tests;

public class ResolveSteamUrlTests
{
    private readonly ResolveSteamUrl _listener;
    private readonly MethodInfo _method;

    public ResolveSteamUrlTests()
    {
        _listener = new(new Mock<IConnection>().Object,
            new Config
            {
                Parameters = new()
                {
                    {"STEAM_KEY", File.ReadAllText("Tokens/SteamToken.txt")},
                },
            },
            new NullLogger<BaseRabbitListenerWithResponse<string, ulong?>>(),
            new());
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
    [TestCase("     ", ExpectedResult = null)]
    [TestCase("\r\n", ExpectedResult = null)]
    [TestCase("http://localhost:15672/#/queues/%2F/test", ExpectedResult = null)]
    public async Task<ulong?> Test1(string value)
    {
        ulong? result = await (_method.Invoke(_listener, new object?[] {value}) as Task<ulong?>);
        return result;
    }
}