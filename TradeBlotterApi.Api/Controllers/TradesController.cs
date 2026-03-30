using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeBlotterApi.Api.Models;
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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TradeResponse>> GetById(Guid id)
    {
        var trade = await _db.Trades
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => ToResponse(t))
            .SingleOrDefaultAsync();

        return trade is null ? NotFound() : Ok(trade);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TradeResponse>>> Get([FromQuery] TradeQueryRequest request)
    {
        var query = _db.Trades.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Desk))
            query = query.Where(t => EF.Functions.ILike(t.Desk, ToContainsPattern(request.Desk)));

        if (!string.IsNullOrWhiteSpace(request.Trader))
            query = query.Where(t => EF.Functions.ILike(t.Trader, ToContainsPattern(request.Trader)));

        if (!string.IsNullOrWhiteSpace(request.Product))
            query = query.Where(t => EF.Functions.ILike(t.Product, ToContainsPattern(request.Product)));

        if (!string.IsNullOrWhiteSpace(request.Symbol))
            query = query.Where(t => EF.Functions.ILike(t.Symbol, ToContainsPattern(request.Symbol)));

        if (request.From is { } from)
        {
            var fromUtc = DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            query = query.Where(t => t.TradeTimeUtc >= fromUtc);
        }

        if (request.To is { } to)
        {
            var toExclusiveUtc = DateTime.SpecifyKind(to.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc).AddDays(1);
            query = query.Where(t => t.TradeTimeUtc < toExclusiveUtc);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.TradeTimeUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => ToResponse(t))
            .ToListAsync();

        return Ok(new PagedResponse<TradeResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }

    [HttpPost]
    public async Task<ActionResult<TradeResponse>> Create([FromBody] TradeCreateRequest request)
    {
        var trade = new Trade
        {
            TradeTimeUtc = request.TradeTimeUtc?.ToUniversalTime() ?? DateTime.UtcNow,
            Desk = request.Desk.Trim(),
            Product = request.Product.Trim(),
            Symbol = request.Symbol.Trim(),
            Side = request.Side.Trim().ToUpperInvariant(),
            Quantity = request.Quantity,
            Price = request.Price,
            Counterparty = request.Counterparty.Trim(),
            Trader = request.Trader.Trim()
        };

        _db.Trades.Add(trade);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = trade.Id }, ToResponse(trade));
    }

    private static TradeResponse ToResponse(Trade trade) =>
        new()
        {
            Id = trade.Id,
            TradeTimeUtc = trade.TradeTimeUtc,
            Desk = trade.Desk,
            Product = trade.Product,
            Symbol = trade.Symbol,
            Side = trade.Side,
            Quantity = trade.Quantity,
            Price = trade.Price,
            Counterparty = trade.Counterparty,
            Trader = trade.Trader
        };

    private static string ToContainsPattern(string value)
    {
        var escaped = value
            .Trim()
            .Replace(@"\", @"\\", StringComparison.Ordinal)
            .Replace("%", @"\%", StringComparison.Ordinal)
            .Replace("_", @"\_", StringComparison.Ordinal);

        return $"%{escaped}%";
    }
}
