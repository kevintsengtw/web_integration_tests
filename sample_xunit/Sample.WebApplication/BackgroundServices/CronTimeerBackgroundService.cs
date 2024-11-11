using Cronos;
using Sgbj.Cron;

namespace Sample.WebApplication.BackgroundServices;

/// <summary>
/// class CronTimeerBackgroundService
/// </summary>
public class CronTimeerBackgroundService : BackgroundService
{
    private readonly ILogger<CronTimeerBackgroundService> _logger;

    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CronTimeerBackgroundService"/> class
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="timeProvider">The timeProvider</param>
    public CronTimeerBackgroundService(ILogger<CronTimeerBackgroundService> logger, TimeProvider timeProvider)
    {
        this._logger = logger;
        this._timeProvider = timeProvider;
    }

    /// <summary>
    /// Executes the stopping token
    /// </summary>
    /// <param name="stoppingToken">The stopping token</param>
    /// <exception cref="NotImplementedException"></exception>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger.LogInformation(
            "{DateTimeNow} [{MachineName}] {TypeName} is starting.",
            $"{this._timeProvider.GetLocalNow().DateTime:yyyy-MM-dd HH:mm:ss}",
            Environment.MachineName,
            this.GetType().Name);

        // cron expression 每個偶數分數裡的每 12 秒
        var cronExpression = CronExpression.Parse("*/12 */2 * * * *", CronFormat.IncludeSeconds);
        
        // 使用 CronTimer
        using var cronTimer = new CronTimer(cronExpression, TimeZoneInfo.Local);

        while (await cronTimer.WaitForNextTickAsync(stoppingToken) && stoppingToken.IsCancellationRequested is false)
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