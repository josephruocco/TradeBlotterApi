using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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