namespace TradeBlotterApi.Api.Models;

public sealed class TradeResponse
{
    public Guid Id { get; init; }
    public DateTime TradeTimeUtc { get; init; }
    public string Desk { get; init; } = string.Empty;
    public string Product { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public string Side { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public string Counterparty { get; init; } = string.Empty;
    public string Trader { get; init; } = string.Empty;
}
