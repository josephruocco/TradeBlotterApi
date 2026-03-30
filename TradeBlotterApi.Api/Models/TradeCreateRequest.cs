using System.ComponentModel.DataAnnotations;

namespace TradeBlotterApi.Api.Models;

public sealed class TradeCreateRequest
{
    public DateTime? TradeTimeUtc { get; set; }

    [Required]
    [StringLength(32)]
    public string Desk { get; set; } = "REPO";

    [Required]
    [StringLength(32)]
    public string Product { get; set; } = "REPO";

    [Required]
    [StringLength(128)]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [StringLength(16)]
    public string Side { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.0000001", "79228162514264337593543950335")]
    public decimal Quantity { get; set; }

    [Range(typeof(decimal), "0.0000001", "79228162514264337593543950335")]
    public decimal Price { get; set; }

    [Required]
    [StringLength(128)]
    public string Counterparty { get; set; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string Trader { get; set; } = string.Empty;
}
