using System;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace AndNetwork9.Shared.Backend;

public interface ITimerService : IHostedService
{
    protected CancellationTokenSource? CancellationTokenSource { get; set; }
    protected TimeSpan Interval { get; }
    protected PeriodicTimer? Timer { get; set; }

    System.Threading.Tasks.Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource = new();
        Timer = new(Interval);
        System.Threading.Tasks.Task.Run(
            async () => await ProcessTimer(CancellationTokenSource.Token).ConfigureAwait(false),
            cancellationToken);
        return System.Threading.Tasks.Task.CompletedTask;
    }

    System.Threading.Tasks.Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = null;
        Timer?.Dispose();
        Timer = null;
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public async System.Threading.Tasks.Task ProcessTimer(CancellationToken cancellationToken = default)
    {
        do
        {
            await Process().ConfigureAwait(false);
        } while (Timer is not null && await Timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false));
    }

    System.Threading.Tasks.Task Process();
}