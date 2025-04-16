namespace AspNetCoreStudy.BackService.UsageMonintor;


public sealed class UsageMonitorService : BackgroundService
{
    private ApiUsageCounter apiUsageCounter { get; set; }

    private readonly ILogger<UsageMonitorService> logger;
    private readonly TimeSpan interval = TimeSpan.FromSeconds(5);

    public UsageMonitorService(ILogger<UsageMonitorService> logger,
                               ApiUsageCounter apiUsageCounter)
    {
        this.logger = logger;
        this.apiUsageCounter = apiUsageCounter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            this.Log();
            await Task.Delay(this.interval, stoppingToken);
        }
    }

    private void Log()
    {
        this.logger.LogInformation($"UsageLog: [" +
            $"IntervalSeconds:{this.interval.TotalSeconds}, " +
            $"ApiUsage:{this.apiUsageCounter.LogAndClearApiUsage()}, " +
            $"].");
    }
}
