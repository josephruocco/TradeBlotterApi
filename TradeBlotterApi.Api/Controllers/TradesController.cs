using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeBlotterApi.Domain.Models;
using TradeBlotterApi.Infrastructure;

namespace TradeBlotterApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TradesController : ControllerBase
{
    private readonly BlotterDbContext _db;

    public TradesController(BlotterDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IEnumerable<Trade>> Get()
        => await _db.Trades.OrderByDescending(t => t.TradeTimeUtc).ToListAsync();

    [HttpPost]
    public async Task<IActionResult> Create(Trade trade)
    {
        _db.Trades.Add(trade);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = trade.Id }, trade);
    }
}