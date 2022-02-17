using Microsoft.Extensions.Hosting;

namespace And9.Lib.Utility;

public abstract class TimerService : IHostedService
{
    protected CancellationTokenSource? CancellationTokenSource { get; set; }
    protected abstract TimeSpan Interval { get; }
    private PeriodicTimer? Timer { get; set; }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource = new();
        Timer = new(Interval);
        Task.Run(
            async () => await ProcessTimer(CancellationTokenSource.Token).ConfigureAwait(false),
            cancellationToken);
        return Task.CompletedTask;
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = null;
        Timer?.Dispose();
        Timer = null;
        return Task.CompletedTask;
    }

    private async Task ProcessTimer(CancellationToken cancellationToken = default)
    {
        while (Timer is not null && await Timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false)) await Process().ConfigureAwait(false);
    }

    protected abstract Task Process();
}