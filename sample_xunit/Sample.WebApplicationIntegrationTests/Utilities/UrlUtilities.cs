using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Sample.WebApplicationIntegrationTests.Utilities;

/// <summary>
/// class UrlUtilities
/// </summary>
public class UrlUtilities
{
    public static QueryString ConvertToQueryString(object obj, bool enableUrlEncode = false)
    {
        var keyValues = from p in obj.GetType().GetProperties()
                        where p.GetValue(obj, null) != null
                        select new KeyValuePair<string, string>
                        (
                            key: $"{p.Name}",
                            value: enableUrlEncode
                                ? $"{HttpUtility.UrlEncode(p.GetValue(obj, null).ToString())}"
                                : p.PropertyType == typeof(DateTime)
                                    ? $"{p.GetValue(obj, null):yyyy-MM-ddTHH:mm:ss.fff}"
                                    : $"{p.GetValue(obj, null)}"
                        );

        var queryBuilder = new QueryBuilder(keyValues);
        var queryString = queryBuilder.ToQueryString();
        return queryString;
    }
}