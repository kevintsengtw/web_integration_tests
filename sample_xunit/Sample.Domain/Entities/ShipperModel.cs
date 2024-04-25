using System.ComponentModel.DataAnnotations;

namespace Sample.Domain.Entities;

/// <summary>
/// class ShipperModel
/// </summary>
public class ShipperModel
{
    /// <summary>
    /// ShipperId
    /// </summary>
    [Required]
    public int ShipperId { get; set; }

    /// <summary>
    /// CompanyName
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(maximumLength: 40)]
    public string CompanyName { get; set; }

    /// <summary>
    /// Phone
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(maximumLength: 24)]
    public string Phone { get; set; }
}