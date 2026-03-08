using Microsoft.AspNetCore.Mvc;
using TradeBlotterApi.Domain.Models;
using TradeBlotterApi.Domain.Services;

namespace TradeBlotterApi.Api.Controllers;

[ApiController]
[Route("api/repo")]
public sealed class RepoController : ControllerBase
{
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
}