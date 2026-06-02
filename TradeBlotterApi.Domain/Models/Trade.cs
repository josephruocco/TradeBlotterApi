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
    public string Status { get; set; } = "PENDING";
    public DateTime? UpdatedAtUtc { get; set; }

    // Repo-specific fields (null for non-repo trades)
    public DateOnly? SettleDate { get; set; }
    public DateOnly? MaturityDate { get; set; }
    public decimal? RepoRate { get; set; }
    public decimal? Haircut { get; set; }
    public decimal? CashProceeds { get; set; }
    public decimal? RepoInterest { get; set; }
    public decimal? TotalRepayment { get; set; }
}