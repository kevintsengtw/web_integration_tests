using Sample.Domain.Repositories;
using Sample.Service.Interface;

namespace Sample.Service.Implements;

/// <summary>
/// class TradeDateService
/// </summary>
public class TradeDateService : ITradeDateService
{
    private readonly TimeProvider _timeProvider;
    
    private readonly ITradeDateApiRepository _tradeDateApiRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeDateService"/> class
    /// </summary>
    /// <param name="timeProvider">The time provider</param>
    /// <param name="tradeDateApiRepository">The trade date api repository</param>
    public TradeDateService(TimeProvider timeProvider,
                            ITradeDateApiRepository tradeDateApiRepository)
    {
        this._timeProvider = timeProvider;
        this._tradeDateApiRepository = tradeDateApiRepository;
    }

    /// <summary>
    /// 取得可交易日期
    /// </summary>
    /// <param name="tradeDate">日期</param>
    /// <param name="count">數量</param>
    /// <returns></returns>
    public async Task<string> GetAsync(DateTime tradeDate, int count)
    {
        var tradeDateModel = await this._tradeDateApiRepository.GetAsync(tradeDate, count);

        var isExists = tradeDateModel.TradeDate.Any(x => $"{x}" == $"{tradeDate:yyyyMMdd}");
        var currentTime = TimeOnly.FromDateTime(this._timeProvider.GetLocalNow().DateTime);
        var pm0300 = new TimeOnly(15, 0, 0);

        var result = isExists is false
            ? tradeDateModel.TradeDate.First()
            : currentTime > pm0300
                ? tradeDateModel.TradeDate.Last()
                : tradeDateModel.TradeDate.First();

        return CovertToDateTime(result);
    }

    /// <summary>
    /// 將八碼數字 (yyyyMMdd) 轉換為 DateTime
    /// </summary>
    /// <param name="dateTimeValue">dateTimeValue</param>
    /// <returns></returns>
    private static string CovertToDateTime(int dateTimeValue)
    {
        var inputStr = dateTimeValue.ToString("00000000");
        
        // 提取西元的年、月、日
        var year = int.Parse(inputStr[..4]);
        var month = int.Parse(inputStr.Substring(4, 2));
        var day = int.Parse(inputStr.Substring(6, 2));

        return $"{year:0000}-{month:00}-{day:00}";
    }
}