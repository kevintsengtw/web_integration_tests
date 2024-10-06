using Sample.Domain.Misc;
using Sample.Domain.Repositories;
using Sample.Service.Interface;

namespace Sample.Service.Implements;

/// <summary>
/// class TradeService
/// </summary>
public class TradeService : ITradeService
{
    private readonly TimeProvider _timeProvider;
    
    private readonly IHolidayRepository _holidayRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeService"/> class
    /// </summary>
    /// <param name="timeProvider">The timeProvider</param>
    /// <param name="holidayRepository">The holidayRepository</param>
    public TradeService(TimeProvider timeProvider, IHolidayRepository holidayRepository)
    {
        this._timeProvider = timeProvider;
        this._holidayRepository = holidayRepository;
    }

    /// <summary>
    /// 現在是否可以交易
    /// </summary>
    /// <returns>The bool</returns>
    public bool IsTradeNow()
    {
        var now = this._timeProvider.GetLocalNow().DateTime;

        var holidays = this._holidayRepository.GetHolidays(now.Year, now.Month);

        if (holidays.Any(x => x.Date == now.Date))
        {
            return false;
        }
        
        var pm0330 = new TimeOnly(15, 30, 0);
        var currentTime = TimeOnly.FromDateTime(now);

        return currentTime < pm0330;
    }
}