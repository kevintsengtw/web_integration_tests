using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Sample.Service.Dto;
using Sample.Service.Interface;
using Sample.WebApplication.Infrastructure.Filters;
using Sample.WebApplication.Infrastructure.Wrapper.Models;
using Sample.WebApplication.Models.InputParameters;
using Sample.WebApplication.Models.OutputModels;

namespace Sample.WebApplication.Controllers;

/// <summary>
/// class ShipperController
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ShipperController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IShipperService _shipperService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperController"/> class
    /// </summary>
    /// <param name="mapper">The mapper</param>
    /// <param name="shipperService">The shipper service</param>
    public ShipperController(IMapper mapper, IShipperService shipperService)
    {
        this._mapper = mapper;
        this._shipperService = shipperService;
    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// 取得所有 Shipper 資料
    /// </summary>
    /// <returns></returns>
    [HttpGet("all")]
    [Produces("application/json", "text/json")]
    [ProducesResponseType(200, Type = typeof(SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>))]
    [ProducesResponseType(400, Type = typeof(FailureResultOutputModel<ResponseMessageOutputModel>))]
    public async Task<IActionResult> GetAllAsync()
    {
        // api/shipper/all.GET

        var shippers = await this._shipperService.GetAllAsync();
        var outputModels = this._mapper.Map<IEnumerable<ShipperOutputModel>>(shippers);

        return this.Ok(outputModels);
    }

    /// <summary>
    /// 取得指定範圍與數量的 Shipper 資料
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [HttpGet("from/{from}/size/{size}")]
    [ParameterValidator("parameter")]
    [Produces("application/json", "text/json")]
    [ProducesResponseType(200, Type = typeof(SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>))]
    [ProducesResponseType(400, Type = typeof(FailureResultOutputModel<ResponseMessageOutputModel>))]
    public async Task<IActionResult> GetCollectionAsync([FromRoute] ShipperPageParameter parameter)
    {
        // api/shipper/{from}/{size}

        var totalCounnt = await this._shipperService.GetTotalCountAsync();
        if (totalCounnt == 0)
        {
            return this.Ok(Enumerable.Empty<ShipperOutputModel>());
        }

        var shippers = await this._shipperService.GetCollectionAsync(parameter.From, parameter.Size);
        var outputModels = this._mapper.Map<IEnumerable<ShipperOutputModel>>(shippers);

        return this.Ok(outputModels);
    }

    /// <summary>
    /// 輸入 companyName 或 phone 查詢相關的 shipper 資料
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [HttpGet("search")]
    [ParameterValidator("parameter")]
    [Produces("application/json", "text/json")]
    [ProducesResponseType(200, Type = typeof(SuccessResultOutputModel<IEnumerable<ShipperOutputModel>>))]
    [ProducesResponseType(400, Type = typeof(FailureResultOutputModel<ResponseMessageOutputModel>))]
    public async Task<IActionResult> SearchAsync([FromQuery] ShipperSearchParameter parameter)
    {
        // api/shipper/search.GET

        var shippers = await this._shipperService.SearchAsync(parameter.CompanyName, parameter.Phone);
        var outputModels = this._mapper.Map<IEnumerable<ShipperOutputModel>>(shippers);

        return this.Ok(outputModels);
    }

    /// <summary>
    /// 取得指定 ShipperId 的 Shipper 資料
    /// </summary>
    /// <param name="parameter">parameter</param>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json", "text/json")]
    [ParameterValidator("parameter")]
    [ProducesResponseType(200, Type = typeof(SuccessResultOutputModel<ShipperOutputModel>))]
    [ProducesResponseType(400, Type = typeof(FailureResultOutputModel<ResponseMessageOutputModel>))]
    public async Task<IActionResult> GetAsync([FromQuery] ShipperIdParameter parameter)
    {
        // api/shiiper.GET

        var exists = await this._shipperService.IsExistsAsync(parameter.ShipperId);
        if (!exists)
        {
            return this.BadRequest(new ResponseMessageOutputModel { Message = "shipper not exists" });
        }

        var shipper = await this._shipperService.GetAsync(parameter.ShipperId);
        var outputModel = this._mapper.Map<ShipperDto, ShipperOutputModel>(shipper);
        return this.Ok(outputModel);
    }

    /// <summary>
    /// 新增 Shipper 資料
    /// </summary>
    /// <param name="parameter">parameter</param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json", "text/json")]
    [ParameterValidator("parameter")]
    [ProducesResponseType(200, Type = typeof(SuccessResultOutputModel<ShipperOutputModel>))]
    [ProducesResponseType(400, Type = typeof(FailureResultOutputModel<ResponseMessageOutputModel>))]
    public async Task<IActionResult> PostAsync([FromBody] ShipperParameter parameter)
    {
        // api/shipper.POST

        var shipper = this._mapper.Map<ShipperParameter, ShipperDto>(parameter);

        var createResult = await this._shipperService.CreateAsync(shipper);
        if (!createResult.Success)
        {
            return this.BadRequest(new ResponseMessageOutputModel { Message = "create failure" });
        }

        return this.Ok(new ResponseMessageOutputModel { Message = "create success" });
    }

    /// <summary>
    /// 修改 Shipper 資料
    /// </summary>
    /// <param name="parameter">parameter</param>
    /// <returns></returns>
    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json", "text/json")]
    [ParameterValidator("parameter")]
    [ProducesResponseType(200, Type = typeof(SuccessResultOutputModel<ShipperOutputModel>))]
    [ProducesResponseType(400, Type = typeof(FailureResultOutputModel<ResponseMessageOutputModel>))]
    public async Task<IActionResult> PutAsync([FromBody] ShipperUpdateParameter parameter)
    {
        // api/shipper.PUT

        var exists = await this._shipperService.IsExistsAsync(parameter.ShipperId);
        if (!exists)
        {
            return this.BadRequest(new ResponseMessageOutputModel { Message = "shipper not exists" });
        }

        var shipper = await this._shipperService.GetAsync(parameter.ShipperId);

        shipper.CompanyName = parameter.CompanyName;
        shipper.Phone = parameter.Phone;

        var updateResult = await this._shipperService.UpdateAsync(shipper);
        if (!updateResult.Success)
        {
            return this.BadRequest(new ResponseMessageOutputModel { Message = "update failure" });
        }

        return this.Ok(new ResponseMessageOutputModel { Message = "update success" });
    }

    /// <summary>
    /// 刪除 Shipper 資料
    /// </summary>
    /// <param name="parameter">parameter</param>
    /// <returns></returns>
    [HttpDelete]
    [Consumes("application/json")]
    [Produces("application/json", "text/json")]
    [ParameterValidator("parameter")]
    [ProducesResponseType(200, Type = typeof(SuccessResultOutputModel<ShipperOutputModel>))]
    [ProducesResponseType(400, Type = typeof(FailureResultOutputModel<ResponseMessageOutputModel>))]
    public async Task<IActionResult> DeleteAsync([FromBody] ShipperIdParameter parameter)
    {
        // api/shipper.DELETE

        var exists = await this._shipperService.IsExistsAsync(parameter.ShipperId);
        if (!exists)
        {
            return this.BadRequest(new ResponseMessageOutputModel { Message = "shipper not exists" });
        }

        var deleteResult = await this._shipperService.DeleteAsync(parameter.ShipperId);
        if (deleteResult.Success)
        {
            return this.Ok(new ResponseMessageOutputModel { Message = "delete success" });
        }

        return this.BadRequest(new ResponseMessageOutputModel { Message = "delete failure" });
    }
}