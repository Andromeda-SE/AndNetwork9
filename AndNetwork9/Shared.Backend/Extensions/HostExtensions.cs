using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace AndNetwork9.Shared.Backend.Extensions
{
    public static class HostExtensions
    {
        public static IHostBuilder ConfigureAndNetConsole(this IHostBuilder builder)
        {
            return builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = false;
                    options.TimestampFormat = "[yy/MM/dd HH:mm:ss.fff]\t";
                    options.ColorBehavior = LoggerColorBehavior.Enabled;
                    options.UseUtcTimestamp = true;
                });
                logging.SetMinimumLevel(
#if DEBUG
LogLevel.Debug
#else 
LogLevel.Information
#endif
);
            });
        }
    }
}