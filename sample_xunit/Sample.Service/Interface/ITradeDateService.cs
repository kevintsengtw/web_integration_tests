namespace Sample.Service.Interface;

/// <summary>
/// interface ITradeDateService
/// </summary>
public interface ITradeDateService
{
    /// <summary>
    /// 取得可交易日期
    /// </summary>
    /// <param name="tradeDate">日期</param>
    /// <param name="count">數量</param>
    /// <returns></returns>
    Task<string> GetAsync(DateTime tradeDate, int count);
}