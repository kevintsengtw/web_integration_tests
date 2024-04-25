using Sample.Domain.Entities;
using Sample.Domain.Misc;

namespace Sample.Domain.Repositories;

/// <summary>
/// interface IShipperRepository
/// </summary>
public interface IShipperRepository
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
    Task<ShipperModel> GetAsync(int shipperId);

    /// <summary>
    /// 取得 Shipper 的資料總數
    /// </summary>
    /// <returns></returns>
    Task<int> GetTotalCountAsync();

    /// <summary>
    /// 取得所有 Shipper 資料
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<ShipperModel>> GetAllAsync();

    /// <summary>
    /// 取得所有 Shipper 資料 (分頁)
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="size">The size.</param>
    /// <returns></returns>
    Task<IEnumerable<ShipperModel>> GetCollectionAsync(int from, int size);

    /// <summary>
    /// 以 CompanyName or Phone 查詢符合條件的資料
    /// </summary>
    /// <param name="companyName">Name of the company.</param>
    /// <param name="phone">The phone.</param>
    /// <returns></returns>
    Task<IEnumerable<ShipperModel>> SearchAsync(string companyName, string phone);

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    Task<IResult> CreateAsync(ShipperModel model);

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="model">The mdoel.</param>
    /// <returns></returns>
    Task<IResult> UpdateAsync(ShipperModel model);

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    Task<IResult> DeleteAsync(int shipperId);
}