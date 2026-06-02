using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeBlotterApi.Api.Controllers;
using TradeBlotterApi.Api.Models;
using TradeBlotterApi.Domain.Models;
using TradeBlotterApi.Infrastructure;

namespace TradeBlotterApi.Tests;

public sealed class TradesControllerTests
{
    [Fact]
    public async Task Get_ReturnsPagedTradesOrderedDescending()
    {
        await using var db = CreateDbContext();
        db.Trades.AddRange(
            new Trade
            {
                Desk = "REPO",
                Product = "REPO",
                Symbol = "UST10",
                Side = "BUY",
                Quantity = 100m,
                Price = 99m,
                Counterparty = "A",
                Trader = "Joe",
                TradeTimeUtc = new DateTime(2026, 3, 28, 12, 0, 0, DateTimeKind.Utc)
            },
            new Trade
            {
                Desk = "REPO",
                Product = "REPO",
                Symbol = "UST5",
                Side = "SELL",
                Quantity = 50m,
                Price = 101m,
                Counterparty = "B",
                Trader = "Joe",
                TradeTimeUtc = new DateTime(2026, 3, 29, 12, 0, 0, DateTimeKind.Utc)
            });
        await db.SaveChangesAsync();

        var controller = new TradesController(db);

        var actionResult = await controller.Get(new TradeQueryRequest { Page = 1, PageSize = 1 });

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<PagedResponse<TradeResponse>>(ok.Value);

        Assert.Equal(2, response.TotalCount);
        Assert.Single(response.Items);
        Assert.Equal("UST5", response.Items[0].Symbol);
    }

    [Fact]
    public async Task Create_PersistsTradeAndReturnsCreatedAtGetById()
    {
        await using var db = CreateDbContext();
        var controller = new TradesController(db);

        var request = new TradeCreateRequest
        {
            Desk = " repo ",
            Product = "repo ",
            Symbol = " UST2 ",
            Side = "buy",
            Quantity = 250m,
            Price = 99.75m,
            Counterparty = " GS ",
            Trader = " Bob "
        };

        var actionResult = await controller.Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        Assert.Equal(nameof(TradesController.GetById), created.ActionName);

        var response = Assert.IsType<TradeResponse>(created.Value);
        var savedTrade = await db.Trades.SingleAsync();

        Assert.Equal(savedTrade.Id, response.Id);
        Assert.Equal("repo", savedTrade.Desk);
        Assert.Equal("repo", savedTrade.Product);
        Assert.Equal("UST2", savedTrade.Symbol);
        Assert.Equal("BUY", savedTrade.Side);
        Assert.Equal("GS", savedTrade.Counterparty);
        Assert.Equal("Bob", savedTrade.Trader);
    }

    [Fact]
    public async Task Update_AmendsTrade()
    {
        await using var db = CreateDbContext();
        var trade = new Trade
        {
            Desk = "REPO", Product = "REPO", Symbol = "UST10",
            Side = "BUY", Quantity = 100m, Price = 99m,
            Counterparty = "A", Trader = "Joe"
        };
        db.Trades.Add(trade);
        await db.SaveChangesAsync();

        var controller = new TradesController(db);

        var actionResult = await controller.Update(trade.Id, new TradeUpdateRequest
        {
            Quantity = 200m,
            Price = 100.5m
        });

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<TradeResponse>(ok.Value);
        Assert.Equal(200m, response.Quantity);
        Assert.Equal(100.5m, response.Price);
        Assert.Equal("UST10", response.Symbol);
        Assert.NotNull(response.UpdatedAtUtc);
    }

    [Fact]
    public async Task Update_ReturnsNotFoundForMissingTrade()
    {
        await using var db = CreateDbContext();
        var controller = new TradesController(db);

        var actionResult = await controller.Update(Guid.NewGuid(), new TradeUpdateRequest { Price = 1m });

        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async Task Update_RejectsCancelledTrade()
    {
        await using var db = CreateDbContext();
        var trade = new Trade
        {
            Desk = "REPO", Product = "REPO", Symbol = "UST10",
            Side = "BUY", Quantity = 100m, Price = 99m,
            Counterparty = "A", Trader = "Joe", Status = "CANCELLED"
        };
        db.Trades.Add(trade);
        await db.SaveChangesAsync();

        var controller = new TradesController(db);

        var actionResult = await controller.Update(trade.Id, new TradeUpdateRequest { Price = 50m });

        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task Cancel_SoftDeletesTrade()
    {
        await using var db = CreateDbContext();
        var trade = new Trade
        {
            Desk = "REPO", Product = "REPO", Symbol = "UST10",
            Side = "BUY", Quantity = 100m, Price = 99m,
            Counterparty = "A", Trader = "Joe"
        };
        db.Trades.Add(trade);
        await db.SaveChangesAsync();

        var controller = new TradesController(db);

        var result = await controller.Cancel(trade.Id);

        Assert.IsType<NoContentResult>(result);
        var updated = await db.Trades.FindAsync(trade.Id);
        Assert.Equal("CANCELLED", updated!.Status);
        Assert.NotNull(updated.UpdatedAtUtc);
    }

    [Fact]
    public async Task Cancel_ReturnsNotFoundForMissingTrade()
    {
        await using var db = CreateDbContext();
        var controller = new TradesController(db);

        var result = await controller.Cancel(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Cancel_IsIdempotent()
    {
        await using var db = CreateDbContext();
        var trade = new Trade
        {
            Desk = "REPO", Product = "REPO", Symbol = "UST10",
            Side = "BUY", Quantity = 100m, Price = 99m,
            Counterparty = "A", Trader = "Joe", Status = "CANCELLED"
        };
        db.Trades.Add(trade);
        await db.SaveChangesAsync();

        var controller = new TradesController(db);

        var result = await controller.Cancel(trade.Id);

        Assert.IsType<NoContentResult>(result);
    }

    private static BlotterDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BlotterDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BlotterDbContext(options);
    }
}
