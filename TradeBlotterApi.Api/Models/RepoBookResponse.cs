namespace TradeBlotterApi.Api.Models;

public sealed class RepoBookResponse
{
    public Guid TradeId { get; init; }
    public DateTime TradeTimeUtc { get; init; }
    public string Symbol { get; init; } = string.Empty;
    public string Side { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public string Counterparty { get; init; } = string.Empty;
    public string Trader { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;

    // Repo economics
    public DateOnly SettleDate { get; init; }
    public DateOnly MaturityDate { get; init; }
    public int Days { get; init; }
    public decimal RepoRate { get; init; }
    public decimal Haircut { get; init; }
    public decimal CashProceeds { get; init; }
    public decimal RepoInterest { get; init; }
    public decimal TotalRepayment { get; init; }
}
