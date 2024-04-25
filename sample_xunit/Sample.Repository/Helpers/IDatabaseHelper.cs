using System.Data;

namespace Sample.Repository.Helpers;

/// <summary>
/// Interface IDatabaseHelper
/// </summary>
public interface IDatabaseHelper
{
    /// <summary>
    /// Gets the connection.
    /// </summary>
    /// <returns>IDbConnection.</returns>
    IDbConnection GetConnection();
}