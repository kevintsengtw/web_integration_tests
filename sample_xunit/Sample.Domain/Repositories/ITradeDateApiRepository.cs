using Sample.Domain.Entities;

namespace Sample.Domain.Repositories;

/// <summary>
/// interface ITradeDateApiRepository
/// </summary>
public interface ITradeDateApiRepository
{
    /// <summary>
    /// 取得可交易日期
    /// </summary>
    /// <param name="tradeDate">日期</param>
    /// <param name="count">數量</param>
    /// <returns></returns>
    Task<TradeDateModel> GetAsync(DateTime tradeDate, int count = 0);
}