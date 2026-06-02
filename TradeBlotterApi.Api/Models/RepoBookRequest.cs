using System.ComponentModel.DataAnnotations;

namespace TradeBlotterApi.Api.Models;

public sealed class RepoBookRequest
{
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

    [Range(typeof(decimal), "0", "0.9999999")]
    public decimal Haircut { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal RepoRate { get; set; }

    public DateOnly SettleDate { get; set; }
    public DateOnly MaturityDate { get; set; }
}
