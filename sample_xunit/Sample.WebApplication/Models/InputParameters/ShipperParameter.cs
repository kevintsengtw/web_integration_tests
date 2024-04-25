using System.ComponentModel.DataAnnotations;

namespace Sample.WebApplication.Models.InputParameters;

/// <summary>
/// class ShipperParameter
/// </summary>
public class ShipperParameter
{
    /// <summary>
    /// CompanyName
    /// </summary>
    [Required]
    [StringLength(maximumLength: 40)]
    public string CompanyName { get; set; }

    /// <summary>
    /// Phone
    /// </summary>
    [Required]
    [StringLength(maximumLength: 24)]
    public string Phone { get; set; }
}