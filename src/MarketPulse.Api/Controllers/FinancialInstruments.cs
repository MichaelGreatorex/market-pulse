using MarketPulse.Api.DTOs;
using MarketPulse.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinancialInstrumentsController : ControllerBase
{
    private readonly FinancialInstrumentService _financialInstrumentService;

    public FinancialInstrumentsController(
        FinancialInstrumentService financialInstrumentService)
    {
        _financialInstrumentService = financialInstrumentService;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var instruments =
            await _financialInstrumentService.GetAllAsync();

        return Ok(instruments);
    }

    [HttpGet("{ticker}")]
    public async Task<ActionResult<FinancialInstrumentDto>> GetByTicker(string ticker)
    {
        var instrument = await _financialInstrumentService.GetByTickerAsync(ticker);

        if (instrument is null)
        {
            return NotFound();
        }

        return Ok(instrument);
    }
}