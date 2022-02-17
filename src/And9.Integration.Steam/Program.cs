using And9.Lib.Utility;

namespace And9.Integration.Steam;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAndNetConsole()
            .ConfigureServices((context, collection) => Startup.ConfigureServices(collection, context.Configuration))
            .UseDefaultServiceProvider(options =>
            {
#if DEBUG
                options.ValidateScopes = true;
#else
                options.ValidateScopes = false;
#endif
            });
    }
}