using System.ComponentModel.DataAnnotations;

namespace TradeBlotterApi.Api.Models;

public sealed class TradeUpdateRequest
{
    [StringLength(32)]
    public string? Desk { get; set; }

    [StringLength(32)]
    public string? Product { get; set; }

    [StringLength(128)]
    public string? Symbol { get; set; }

    [StringLength(16)]
    public string? Side { get; set; }

    [Range(typeof(decimal), "0.0000001", "79228162514264337593543950335")]
    public decimal? Quantity { get; set; }

    [Range(typeof(decimal), "0.0000001", "79228162514264337593543950335")]
    public decimal? Price { get; set; }

    [StringLength(128)]
    public string? Counterparty { get; set; }

    [StringLength(128)]
    public string? Trader { get; set; }
}
