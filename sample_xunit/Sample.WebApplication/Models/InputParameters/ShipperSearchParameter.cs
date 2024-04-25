using System.ComponentModel;

namespace Sample.WebApplication.Models.InputParameters;

/// <summary>
/// class ShipperSearchParameter
/// </summary>
public class ShipperSearchParameter
{
    /// <summary>
    /// CompanyName
    /// </summary>
    [DisplayName("CompanyName")]
    public string CompanyName { get; set; }

    /// <summary>
    /// Phone
    /// </summary>
    [DisplayName("Phone")]
    public string Phone { get; set; }
}