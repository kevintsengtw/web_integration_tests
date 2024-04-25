using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Sample.WebApplication.Models.InputParameters;

/// <summary>
/// class ShipperPageParameter
/// </summary>
public class ShipperPageParameter
{
    /// <summary>
    /// From
    /// </summary>
    [Required]
    [DisplayName("from")]
    [FromRoute(Name = "from")]
    public int From { get; set; }

    /// <summary>
    /// Size
    /// </summary>
    [Required]
    [DisplayName("size")]
    [FromRoute(Name = "size")]
    public int Size { get; set; }
}