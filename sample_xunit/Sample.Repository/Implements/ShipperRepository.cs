using Dapper;
using Sample.Domain.Entities;
using Sample.Domain.Misc;
using Sample.Domain.Repositories;
using Sample.Domain.Validation;
using Sample.Repository.Helpers;
using Throw;

// ReSharper disable PossibleMultipleEnumeration

namespace Sample.Repository.Implements;

/// <summary>
/// Class ShipperRepository
/// </summary>
public class ShipperRepository : IShipperRepository
{
    private readonly IDatabaseHelper _databaseHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipperRepository"/> class.
    /// </summary>
    /// <param name="databaseHelper">The databaseHelper.</param>
    public ShipperRepository(IDatabaseHelper databaseHelper)
    {
        this._databaseHelper = databaseHelper;
    }

    /// <summary>
    /// 以 ShipperId 查詢資料是否存在
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    public async Task<bool> IsExistsAsync(int shipperId)
    {
        shipperId.Throw().IfLessThanOrEqualTo(0);

        using var conn = this._databaseHelper.GetConnection();

        const string sqlCommand = """
                                  select count(ShipperId) from Shippers
                                  where ShipperId = @ShipperId
                                  """;

        var parameters = new DynamicParameters();
        parameters.Add("ShipperId", shipperId);

        var result = await conn.QueryFirstOrDefaultAsync<int>(
            sql: sqlCommand,
            param: parameters);

        return result > 0;
    }

    /// <summary>
    /// 以 ShipperId 取得資料
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    public async Task<ShipperModel> GetAsync(int shipperId)
    {
        shipperId.Throw().IfLessThanOrEqualTo(0);

        using var conn = this._databaseHelper.GetConnection();

        const string sqlCommand = """
                                  select ShipperId, CompanyName, Phone from Shippers
                                  where ShipperId = @ShipperId
                                  """;

        var parameters = new DynamicParameters();
        parameters.Add("ShipperId", shipperId);

        var result = await conn.QueryFirstOrDefaultAsync<ShipperModel>(
            sql: sqlCommand,
            param: parameters);

        return result;
    }

    /// <summary>
    /// 取得 Shipper 的資料總數
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetTotalCountAsync()
    {
        using var conn = this._databaseHelper.GetConnection();

        const string sqlCommand = "select count(ShipperId) from Shippers ";

        var result = await conn.QueryFirstOrDefaultAsync<int>(sql: sqlCommand);

        return result;
    }

    /// <summary>
    /// 取得所有 Shipper 資料
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<ShipperModel>> GetAllAsync()
    {
        using var conn = this._databaseHelper.GetConnection();

        const string sqlCommand = """
                                  select ShipperId, CompanyName, Phone from Shippers
                                  order by ShipperId ASC
                                  """;

        var result = await conn.QueryAsync<ShipperModel>(sql: sqlCommand);

        return result;
    }

    /// <summary>
    /// 取得所有 Shipper 資料 (分頁)
    /// </summary>
    /// <param name="from"></param>
    /// <param name="size">The size.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// from
    /// or
    /// size
    /// </exception>
    public async Task<IEnumerable<ShipperModel>> GetCollectionAsync(int from, int size)
    {
        from.Throw().IfLessThanOrEqualTo(0);
        size.Throw().IfLessThanOrEqualTo(0);

        var totalCount = await this.GetTotalCountAsync();
        if (totalCount.Equals(0))
        {
            return Enumerable.Empty<ShipperModel>();
        }

        if (from > totalCount)
        {
            return Enumerable.Empty<ShipperModel>();
        }

        const string sqlCommand = """
                                  select ShipperId, CompanyName, Phone
                                  from Shippers
                                  Order by ShipperId ASC
                                  OFFSET @OFFSET ROWS
                                  FETCH NEXT @FETCH ROWS ONLY;
                                  """;

        var pageSize = size is < 0 or > 100 ? 10 : size;
        var start = from <= 0 ? 1 : from;

        var parameters = new DynamicParameters();

        parameters.Add("OFFSET", start - 1);
        parameters.Add("FETCH", pageSize);

        using var conn = this._databaseHelper.GetConnection();

        var query = await conn.QueryAsync<ShipperModel>(sql: sqlCommand, param: parameters);

        var models = query.Any() ? query : Enumerable.Empty<ShipperModel>();
        return models;
    }

    /// <summary>
    /// 以 CompanyName or Phone 查詢符合條件的資料
    /// </summary>
    /// <param name="companyName">Name of the company.</param>
    /// <param name="phone">The phone.</param>
    /// <returns></returns>
    public async Task<IEnumerable<ShipperModel>> SearchAsync(string companyName, string phone)
    {
        if (string.IsNullOrWhiteSpace(companyName) && string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("companyName 與 phone 不可都為空白");
        }

        var totalCount = await this.GetTotalCountAsync();
        if (totalCount.Equals(0))
        {
            return Enumerable.Empty<ShipperModel>();
        }

        var parameters = new DynamicParameters();
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(companyName))
        {
            parameters.Add("CompanyName", companyName.Trim());
            conditions.Add("CompanyName like CONCAT(N'%',@CompanyName,'%') ");
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            parameters.Add("Phone", phone.Trim());
            conditions.Add("Phone like CONCAT(N'%',@Phone,'%') ");
        }

        var sqlCommand = """
                         select ShipperId, CompanyName, Phone
                         from Shippers
                         {{conditions}}
                         Order by ShipperId ASC
                         """;

        if (conditions.Count != 0)
        {
            sqlCommand = sqlCommand.Replace("{{conditions}}", $"where {string.Join(" and ", conditions)}");
        }

        using var conn = this._databaseHelper.GetConnection();

        var query = await conn.QueryAsync<ShipperModel>(sql: sqlCommand, param: parameters);

        var models = query.Any() ? query : Enumerable.Empty<ShipperModel>();
        return models;
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    public async Task<IResult> CreateAsync(ShipperModel model)
    {
        ModelValidator.Validate(model, nameof(model));

        using var conn = this._databaseHelper.GetConnection();

        const string sqlCommand = "Insert into Shippers (CompanyName, Phone) Values (@CompanyName, @Phone)";

        var executeResult = await conn.ExecuteAsync(sql: sqlCommand, param: model);

        IResult result = new Result(false);

        if (executeResult.Equals(1))
        {
            result.Success = true;
            result.AffectRows = executeResult;
            return result;
        }

        result.Message = "資料新增錯誤";
        return result;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    public async Task<IResult> UpdateAsync(ShipperModel model)
    {
        ModelValidator.Validate(model, nameof(model));

        using var conn = this._databaseHelper.GetConnection();

        const string sqlCommand = """
                                  UPDATE Shippers SET
                                  CompanyName = @CompanyName,
                                  Phone = @Phone
                                  WHERE ShipperID = @ShipperID
                                  """;

        var executeResult = await conn.ExecuteAsync(sql: sqlCommand, param: model);

        IResult result = new Result(false);

        if (executeResult.Equals(1))
        {
            result.Success = true;
            result.AffectRows = executeResult;
            return result;
        }

        result.Message = "資料更新錯誤";
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

        using var conn = this._databaseHelper.GetConnection();

        const string sqlCommand = """
                                  DELETE FROM Shippers
                                  WHERE ShipperID = @ShipperID
                                  """;

        var parameters = new DynamicParameters();
        parameters.Add("ShipperID", shipperId);

        var executeResult = await conn.ExecuteAsync(sql: sqlCommand, param: parameters);

        IResult result = new Result(false);

        if (executeResult.Equals(1))
        {
            result.Success = true;
            result.AffectRows = executeResult;
            return result;
        }

        result.Message = "資料刪除錯誤";
        return result;
    }
}