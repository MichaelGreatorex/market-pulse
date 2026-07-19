using MarketPulse.Api.Clients;
using Microsoft.AspNetCore.Mvc;

namespace MarketPulse.Api.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly IMarketDataClient _client;

    public DebugController(IMarketDataClient client)
    {
        _client = client;
    }

    [HttpGet("quote/{ticker}")]
    public async Task<IActionResult> GetQuote(string ticker)
    {
        var quote = await _client.GetQuoteAsync(ticker);

        return Ok(quote);
    }
}