using System.ComponentModel.DataAnnotations;

namespace TradeBlotterApi.Api.Models;

public sealed class TradeQueryRequest
{
    public string? Desk { get; set; }
    public string? Trader { get; set; }
    public string? Product { get; set; }
    public string? Symbol { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 50;
}
