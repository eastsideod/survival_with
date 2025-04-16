namespace AspNetCoreStudy.BackService.UsageMonintor;

public sealed class ApiUsageCounter
{
    private volatile int useCount = 0;

    public void Increment()
    {
        Interlocked.Increment(ref this.useCount);
    }

    public string LogAndClearApiUsage()
    {
        var log = $"useCount:{this.useCount}";
        Interlocked.Exchange(ref this.useCount, 0);

        return log;
    }
}
