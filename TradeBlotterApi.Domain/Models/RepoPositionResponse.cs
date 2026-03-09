namespace TradeBlotterApi.Domain.Models;

public sealed class RepoPositionResponse
{
    public string Symbol { get; set; } = "";
    public decimal NetQuantity { get; set; }
    public decimal AveragePrice { get; set; }
    public int TradeCount { get; set; }
}