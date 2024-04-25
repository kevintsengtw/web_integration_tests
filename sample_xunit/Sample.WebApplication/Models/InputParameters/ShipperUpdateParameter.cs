using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sample.WebApplication.Models.InputParameters;

/// <summary>
/// class ShipperUpdateParameter
/// </summary>
public class ShipperUpdateParameter
{
    [Required]
    [DisplayName("ShipperId")]
    public int ShipperId { get; set; }

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