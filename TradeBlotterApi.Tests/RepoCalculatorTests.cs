using TradeBlotterApi.Domain.Models;
using TradeBlotterApi.Domain.Services;

namespace TradeBlotterApi.Tests;

public sealed class RepoCalculatorTests
{
    [Fact]
    public void Quote_ComputesExpectedCashflows()
    {
        var request = new RepoQuoteRequest
        {
            Price = 99.5m,
            Quantity = 1_000_000m,
            Haircut = 0.02m,
            RepoRate = 0.05m,
            SettleDate = new DateOnly(2026, 3, 30),
            MaturityDate = new DateOnly(2026, 4, 6)
        };

        var result = RepoCalculator.Quote(request);

        Assert.Equal(97_510_000m, result.CashProceeds);
        Assert.Equal(94_801.38888888888888888888889m, result.RepoInterest);
        Assert.Equal(97_604_801.38888888888888888888889m, result.TotalRepayment);
        Assert.Equal(7, result.Days);
    }
}
