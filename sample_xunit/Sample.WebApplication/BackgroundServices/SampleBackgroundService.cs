namespace Sample.WebApplication.BackgroundServices;

/// <summary>
/// class SampleBackgroundService
/// </summary>
public class SampleBackgroundService : BackgroundService
{
    /// <summary>
    /// 執行間隔時間 60 sec
    /// </summary>
    private static readonly TimeSpan IntervalTime = TimeSpan.FromSeconds(60);

    private readonly ILogger<SampleBackgroundService> _logger;

    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleBackgroundService"/> class
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="timeProvider">The timeProvider</param>
    public SampleBackgroundService(ILogger<SampleBackgroundService> logger, TimeProvider timeProvider)
    {
        this._logger = logger;
        this._timeProvider = timeProvider;
    }

    /// <summary>
    /// Executes the stopping token
    /// </summary>
    /// <param name="stoppingToken">The stopping token</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger.LogInformation(
            "{DateTimeNow} [{MachineName}] {TypeName} is starting.",
            $"{this._timeProvider.GetLocalNow().DateTime:yyyy-MM-dd HH:mm:ss}",
            Environment.MachineName,
            this.GetType().Name);

        // 使用 PeriodicTimer
        using PeriodicTimer periodicTimer = new(IntervalTime);

        while (await periodicTimer.WaitForNextTickAsync(stoppingToken) && stoppingToken.IsCancellationRequested is false)
        {
            this._logger.LogInformation(
                "{DateTimeNow} [{MachineName}] {TypeName} Processing.",
                $"{this._timeProvider.GetLocalNow().DateTime:yyyy-MM-dd HH:mm:ss}",
                Environment.MachineName,
                this.GetType().Name);
        }
    }

    /// <summary>
    /// Stops the cancellation token
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation(
            "{DateTimeNow} [{MachineName}] {TypeName} is stopping.",
            $"{this._timeProvider.GetLocalNow().DateTime:yyyy-MM-dd HH:mm:ss}",
            Environment.MachineName,
            this.GetType().Name);
        await base.StopAsync(cancellationToken);
    }
}