namespace Sample.Service.Interface;

/// <summary>
/// interface ITradeService
/// </summary>
public interface ITradeService
{
    /// <summary>
    /// 現在是否可以交易
    /// </summary>
    /// <returns>The bool</returns>
    bool IsTradeNow();
}