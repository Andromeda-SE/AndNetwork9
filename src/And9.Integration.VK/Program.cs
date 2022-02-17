using And9.Lib.Utility;

namespace And9.Integration.VK;

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