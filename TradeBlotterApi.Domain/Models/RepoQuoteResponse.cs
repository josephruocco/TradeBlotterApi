namespace TradeBlotterApi.Domain.Models;

public sealed class RepoQuoteResponse
{
    public decimal CashProceeds { get; set; }
    public decimal RepoInterest { get; set; }
    public decimal TotalRepayment { get; set; }
    public int Days { get; set; }
}