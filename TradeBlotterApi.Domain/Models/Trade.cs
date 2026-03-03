namespace TradeBlotterApi.Domain.Models;

public sealed class Trade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime TradeTimeUtc { get; set; } = DateTime.UtcNow;

    public string Desk { get; set; } = "REPO";
    public string Product { get; set; } = "REPO";
    public string Symbol { get; set; } = "";
    public string Side { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public string Counterparty { get; set; } = "";
    public string Trader { get; set; } = "";
}