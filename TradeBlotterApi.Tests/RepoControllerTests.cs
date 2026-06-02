using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeBlotterApi.Api.Controllers;
using TradeBlotterApi.Api.Models;
using TradeBlotterApi.Infrastructure;

namespace TradeBlotterApi.Tests;

public sealed class RepoControllerTests
{
    [Fact]
    public async Task Book_PersistsTradeWithCashflows()
    {
        await using var db = CreateDbContext();
        var controller = new RepoController(db);

        var request = new RepoBookRequest
        {
            Symbol = " UST10 ",
            Side = "lend",
            Quantity = 1_000_000m,
            Price = 99.5m,
            Counterparty = " JPM ",
            Trader = " Joe ",
            Haircut = 0.02m,
            RepoRate = 0.05m,
            SettleDate = new DateOnly(2026, 3, 30),
            MaturityDate = new DateOnly(2026, 4, 6)
        };

        var actionResult = await controller.Book(request);

        var created = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var response = Assert.IsType<RepoBookResponse>(created.Value);

        Assert.Equal("UST10", response.Symbol);
        Assert.Equal("LEND", response.Side);
        Assert.Equal("CONFIRMED", response.Status);
        Assert.Equal(7, response.Days);
        Assert.Equal(97_510_000m, response.CashProceeds);
        Assert.Equal(0.02m, response.Haircut);
        Assert.Equal(0.05m, response.RepoRate);
        Assert.True(response.RepoInterest > 0);
        Assert.True(response.TotalRepayment > response.CashProceeds);

        // Verify persisted in DB
        var saved = await db.Trades.SingleAsync();
        Assert.Equal("REPO", saved.Product);
        Assert.Equal("REPO", saved.Desk);
        Assert.Equal(new DateOnly(2026, 3, 30), saved.SettleDate);
        Assert.Equal(new DateOnly(2026, 4, 6), saved.MaturityDate);
        Assert.Equal(97_510_000m, saved.CashProceeds);
    }

    [Fact]
    public async Task Book_RejectsMaturityBeforeSettle()
    {
        await using var db = CreateDbContext();
        var controller = new RepoController(db);

        var request = new RepoBookRequest
        {
            Symbol = "UST10",
            Side = "LEND",
            Quantity = 1_000_000m,
            Price = 99.5m,
            Counterparty = "JPM",
            Trader = "Joe",
            Haircut = 0.02m,
            RepoRate = 0.05m,
            SettleDate = new DateOnly(2026, 4, 6),
            MaturityDate = new DateOnly(2026, 3, 30)
        };

        var actionResult = await controller.Book(request);

        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task Book_TradeShowsInPositions()
    {
        await using var db = CreateDbContext();
        var controller = new RepoController(db);

        await controller.Book(new RepoBookRequest
        {
            Symbol = "UST5",
            Side = "BUY",
            Quantity = 500_000m,
            Price = 100m,
            Counterparty = "GS",
            Trader = "Joe",
            Haircut = 0.01m,
            RepoRate = 0.04m,
            SettleDate = new DateOnly(2026, 4, 1),
            MaturityDate = new DateOnly(2026, 4, 8)
        });

        var positionsResult = await controller.GetPositions();

        var ok = Assert.IsType<OkObjectResult>(positionsResult.Result);
        var positions = Assert.IsAssignableFrom<IEnumerable<Domain.Models.RepoPositionResponse>>(ok.Value);
        var pos = Assert.Single(positions);
        Assert.Equal("UST5", pos.Symbol);
        Assert.Equal(500_000m, pos.NetQuantity);
    }

    private static BlotterDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BlotterDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BlotterDbContext(options);
    }
}
