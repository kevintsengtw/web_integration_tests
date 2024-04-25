using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sample.WebApplication.Models.InputParameters;

/// <summary>
/// class ShipperIdParameter
/// </summary>
public class ShipperIdParameter
{
    /// <summary>
    /// ShipperId
    /// </summary>
    [Required]
    [DisplayName("ShipperId")]
    public int ShipperId { get; set; }
}