using MapsterMapper;
using Sample.Domain.Entities;
using Sample.Domain.Misc;
using Sample.Domain.Repositories;
using Sample.Domain.Validation;
using Sample.Service.Dto;
using Sample.Service.Interface;
using Throw;

namespace Sample.Service.Implements;

/// <summary>
/// class ShipperService.
/// </summary>
public class ShipperService : IShipperService
{
    private readonly IMapper _mapper;

    private readonly IShipperRepository _shipperRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperService"/> class.
    /// </summary>
    /// <param name="mapper">The mapper</param>
    /// <param name="shipperRepository">The shipperRepository</param>
    public ShipperService(IMapper mapper, IShipperRepository shipperRepository)
    {
        this._mapper = mapper;
        this._shipperRepository = shipperRepository;
    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// 以 ShipperId 查詢資料是否存在
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    public async Task<bool> IsExistsAsync(int shipperId)
    {
        shipperId.Throw().IfLessThanOrEqualTo(0);
        var exists = await this._shipperRepository.IsExistsAsync(shipperId);
        return exists;
    }

    /// <summary>
    /// 以 ShipperId 查詢資料是否存在
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    public async Task<ShipperDto> GetAsync(int shipperId)
    {
        shipperId.Throw().IfLessThanOrEqualTo(0);

        var exists = await this._shipperRepository.IsExistsAsync(shipperId);
        if (!exists)
        {
            return null;
        }

        var model = await this._shipperRepository.GetAsync(shipperId);
        var shipper = this._mapper.Map<ShipperModel, ShipperDto>(model);
        return shipper;
    }

    /// <summary>
    /// 取得 Shipper 的資料總數
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetTotalCountAsync()
    {
        var totalCount = await this._shipperRepository.GetTotalCountAsync();
        return totalCount;
    }

    /// <summary>
    /// 取得所有 Shipper 資料
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<ShipperDto>> GetAllAsync()
    {
        var models = await this._shipperRepository.GetAllAsync();
        var shippers = this._mapper.Map<IEnumerable<ShipperDto>>(models);
        return shippers;
    }

    /// <summary>
    /// 取得所有 Shipper 資料 (分頁)
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="size">The size.</param>
    /// <returns></returns>
    public async Task<IEnumerable<ShipperDto>> GetCollectionAsync(int @from, int size)
    {
        from.Throw().IfLessThanOrEqualTo(0);
        size.Throw().IfLessThanOrEqualTo(0);

        var totalCount = await this.GetTotalCountAsync();
        if (totalCount.Equals(0))
        {
            return Enumerable.Empty<ShipperDto>();
        }

        if (from > totalCount)
        {
            return Enumerable.Empty<ShipperDto>();
        }

        var models = await this._shipperRepository.GetCollectionAsync(from, size);
        var shippers = this._mapper.Map<IEnumerable<ShipperModel>, IEnumerable<ShipperDto>>(models);
        return shippers;
    }

    /// <summary>
    /// 以 CompanyName or Phone 查詢符合條件的資料
    /// </summary>
    /// <param name="companyName">Name of the company.</param>
    /// <param name="phone">The phone.</param>
    /// <returns></returns>
    public async Task<IEnumerable<ShipperDto>> SearchAsync(string companyName, string phone)
    {
        if (string.IsNullOrWhiteSpace(companyName) && string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("companyName 與 phone 不可都為空白");
        }
        
        var totalCount = await this.GetTotalCountAsync();
        if (totalCount.Equals(0))
        {
            return Enumerable.Empty<ShipperDto>();
        }
        
        var models = await this._shipperRepository.SearchAsync(companyName, phone);
        var shippers = this._mapper.Map<IEnumerable<ShipperDto>>(models);
        return shippers;
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="shipper">The shipper.</param>
    /// <returns></returns>
    public async Task<IResult> CreateAsync(ShipperDto shipper)
    {
        ModelValidator.Validate(shipper, nameof(shipper));

        var model = this._mapper.Map<ShipperDto, ShipperModel>(shipper);
        var result = await this._shipperRepository.CreateAsync(model);
        return result;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="shipper">The shipper.</param>
    /// <returns></returns>
    public async Task<IResult> UpdateAsync(ShipperDto shipper)
    {
        ModelValidator.Validate(shipper, nameof(shipper));

        IResult result = new Result(false);

        var exists = await this._shipperRepository.IsExistsAsync(shipper.ShipperId);
        if (exists is false)
        {
            result.Message = "shipper not exists";
            return result;
        }

        var model = this._mapper.Map<ShipperDto, ShipperModel>(shipper);
        result = await this._shipperRepository.UpdateAsync(model);
        return result;
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    public async Task<IResult> DeleteAsync(int shipperId)
    {
        shipperId.Throw().IfLessThanOrEqualTo(0);

        IResult result = new Result(false);

        var exists = await this._shipperRepository.IsExistsAsync(shipperId);
        if (exists is false)
        {
            result.Message = "shipper not exists";
            return result;
        }

        result = await this._shipperRepository.DeleteAsync(shipperId);
        return result;
    }
}