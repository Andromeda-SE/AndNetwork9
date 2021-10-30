using AndNetwork9.Shared.Backend.Extensions;
using Microsoft.Extensions.Hosting;

namespace AndNetwork9.VK;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureAndNetConsole().ConfigureServices((context, collection) =>
            Startup.ConfigureServices(collection, context.Configuration));
    }
}