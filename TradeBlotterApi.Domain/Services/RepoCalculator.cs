using TradeBlotterApi.Domain.Models;

namespace TradeBlotterApi.Domain.Services;

public static class RepoCalculator
{
    public static RepoQuoteResponse Quote(RepoQuoteRequest req)
    {
        var marketValue = req.Price * req.Quantity;
        var cashProceeds = marketValue * (1 - req.Haircut);
        var days = req.MaturityDate.DayNumber - req.SettleDate.DayNumber;

        var interest = cashProceeds * req.RepoRate * days / 360m;

        return new RepoQuoteResponse
        {
            CashProceeds = cashProceeds,
            RepoInterest = interest,
            TotalRepayment = cashProceeds + interest,
            Days = days
        };
    }
}