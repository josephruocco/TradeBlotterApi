namespace TradeBlotterApi.Domain.Models;

public sealed class RepoQuoteRequest
{
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public decimal RepoRate { get; set; }
    public decimal Haircut { get; set; }
    public DateOnly SettleDate { get; set; }
    public DateOnly MaturityDate { get; set; }
}