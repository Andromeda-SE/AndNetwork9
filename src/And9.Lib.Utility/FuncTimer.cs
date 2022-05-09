namespace And9.Lib.Utility;

[Obsolete("Use Quartz library")]
public class FuncTimer : TimerService
{
    private readonly Func<Task> _func;

    public FuncTimer(Func<Task> func, TimeSpan interval)
    {
        _func = func;
        Interval = interval;
    }

    protected override TimeSpan Interval { get; }
    protected override async Task Process() => await _func().ConfigureAwait(false);
}