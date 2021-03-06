using And9.Lib.Utility;

namespace And9.Gateway.Clan;

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
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
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