using Sample.Domain.Misc;
using Sample.Service.Dto;

namespace Sample.Service.Interface;

/// <summary>
/// interface IShipperService.
/// </summary>
public interface IShipperService
{
    /// <summary>
    /// 以 ShipperId 查詢資料是否存在
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    Task<bool> IsExistsAsync(int shipperId);

    /// <summary>
    /// 以 ShipperId 取得資料
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    Task<ShipperDto> GetAsync(int shipperId);

    /// <summary>
    /// 取得 Shipper 的資料總數
    /// </summary>
    /// <returns></returns>
    Task<int> GetTotalCountAsync();

    /// <summary>
    /// 取得所有 Shipper 資料
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<ShipperDto>> GetAllAsync();

    /// <summary>
    /// 取得所有 Shipper 資料 (分頁)
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="size">The size.</param>
    /// <returns></returns>
    Task<IEnumerable<ShipperDto>> GetCollectionAsync(int from, int size);

    /// <summary>
    /// 以 CompanyName or Phone 查詢符合條件的資料
    /// </summary>
    /// <param name="companyName">Name of the company.</param>
    /// <param name="phone">The phone.</param>
    /// <returns></returns>
    Task<IEnumerable<ShipperDto>> SearchAsync(string companyName, string phone);

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="shipper">The shipper.</param>
    /// <returns></returns>
    Task<IResult> CreateAsync(ShipperDto shipper);

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="shipper">The shipper.</param>
    /// <returns></returns>
    Task<IResult> UpdateAsync(ShipperDto shipper);

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    Task<IResult> DeleteAsync(int shipperId);
}