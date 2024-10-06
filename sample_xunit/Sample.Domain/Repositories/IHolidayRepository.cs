namespace Sample.Domain.Repositories;

public interface IHolidayRepository
{
    /// <summary>
    /// 取得指定年月的假日日期
    /// </summary>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <returns></returns>
    IEnumerable<DateTime> GetHolidays(int year, int month);
}