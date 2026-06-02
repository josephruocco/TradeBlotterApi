using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeBlotterApi.Api.Models;
using TradeBlotterApi.Domain.Models;
using TradeBlotterApi.Domain.Services;
using TradeBlotterApi.Infrastructure;

namespace TradeBlotterApi.Api.Controllers;

[ApiController]
[Route("api/repo")]
public sealed class RepoController : ControllerBase
{
    private readonly BlotterDbContext _db;

    public RepoController(BlotterDbContext db)
    {
        _db = db;
    }

    [HttpPost("quote")]
    public IActionResult Quote([FromBody] RepoQuoteRequest req)
    {
        if (req.MaturityDate <= req.SettleDate)
            return BadRequest("MaturityDate must be after SettleDate.");

        if (req.Quantity <= 0 || req.Price <= 0)
            return BadRequest("Price and Quantity must be positive.");

        if (req.Haircut < 0 || req.Haircut >= 1)
            return BadRequest("Haircut must be between 0 and 1.");

        if (req.RepoRate < 0)
            return BadRequest("RepoRate must be non-negative.");

        var result = RepoCalculator.Quote(req);
        return Ok(result);
    }

    [HttpPost("book")]
    public async Task<ActionResult<RepoBookResponse>> Book([FromBody] RepoBookRequest req)
    {
        if (req.MaturityDate <= req.SettleDate)
            return BadRequest("MaturityDate must be after SettleDate.");

        var quoteInput = new RepoQuoteRequest
        {
            Price = req.Price,
            Quantity = req.Quantity,
            Haircut = req.Haircut,
            RepoRate = req.RepoRate,
            SettleDate = req.SettleDate,
            MaturityDate = req.MaturityDate
        };

        var cashflows = RepoCalculator.Quote(quoteInput);

        var trade = new Trade
        {
            Desk = "REPO",
            Product = "REPO",
            Symbol = req.Symbol.Trim(),
            Side = req.Side.Trim().ToUpperInvariant(),
            Quantity = req.Quantity,
            Price = req.Price,
            Counterparty = req.Counterparty.Trim(),
            Trader = req.Trader.Trim(),
            Status = "CONFIRMED",
            SettleDate = req.SettleDate,
            MaturityDate = req.MaturityDate,
            RepoRate = req.RepoRate,
            Haircut = req.Haircut,
            CashProceeds = cashflows.CashProceeds,
            RepoInterest = cashflows.RepoInterest,
            TotalRepayment = cashflows.TotalRepayment
        };

        _db.Trades.Add(trade);
        await _db.SaveChangesAsync();

        var response = new RepoBookResponse
        {
            TradeId = trade.Id,
            TradeTimeUtc = trade.TradeTimeUtc,
            Symbol = trade.Symbol,
            Side = trade.Side,
            Quantity = trade.Quantity,
            Price = trade.Price,
            Counterparty = trade.Counterparty,
            Trader = trade.Trader,
            Status = trade.Status,
            SettleDate = req.SettleDate,
            MaturityDate = req.MaturityDate,
            Days = cashflows.Days,
            RepoRate = req.RepoRate,
            Haircut = req.Haircut,
            CashProceeds = cashflows.CashProceeds,
            RepoInterest = cashflows.RepoInterest,
            TotalRepayment = cashflows.TotalRepayment
        };

        return CreatedAtAction("GetById", "Trades", new { id = trade.Id }, response);
    }

    [HttpGet("positions")]
    public async Task<ActionResult<IEnumerable<RepoPositionResponse>>> GetPositions()
    {
        var positions = await _db.Trades
            .Where(t => t.Product == "REPO")
            .GroupBy(t => t.Symbol)
            .Select(g => new RepoPositionResponse
            {
                Symbol = g.Key,
                NetQuantity = g.Sum(t => t.Side == "BUY" || t.Side == "BORROW" ? t.Quantity : -t.Quantity),
                AveragePrice = g.Average(t => t.Price),
                TradeCount = g.Count()
            })
            .OrderByDescending(x => x.NetQuantity)
            .ToListAsync();

        return Ok(positions);
    }
}