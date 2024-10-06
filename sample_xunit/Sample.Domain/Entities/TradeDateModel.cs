namespace Sample.Domain.Entities;

/// <summary>
/// class TradeDateModel
/// </summary>
public class TradeDateModel
{
    /// <summary>
    /// 交易日期 (yyyyMMdd)
    /// </summary>
    public IEnumerable<int> TradeDate { get; set; }
}