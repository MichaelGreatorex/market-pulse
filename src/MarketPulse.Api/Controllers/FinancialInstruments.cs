using MarketPulse.Api.DTOs;
using MarketPulse.Api.Models.Pagination;
using MarketPulse.Api.Models.Queries;
using MarketPulse.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MarketPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinancialInstrumentsController : ControllerBase
{
    private readonly FinancialInstrumentService _financialInstrumentService;
    private readonly MarketPriceService _marketPriceService;

    public FinancialInstrumentsController(
        FinancialInstrumentService financialInstrumentService,
        MarketPriceService marketPriceService)
    {
        _financialInstrumentService = financialInstrumentService;
        _marketPriceService = marketPriceService;
    }


    [HttpGet]
    public async Task<ActionResult<PagedResult<FinancialInstrumentDto>>> Get(
        [FromQuery] FinancialInstrumentQuery query)
    {
        var result = await _financialInstrumentService.GetAllAsync(query);

        return Ok(result);
    }

    [HttpGet("{ticker}")]
    public async Task<ActionResult<FinancialInstrumentDto>> GetByTicker(
        [FromRoute] string ticker)
    {
        var instrument = await _financialInstrumentService.GetByTickerAsync(ticker);

        if (instrument is null)
        {
            return NotFound();
        }

        return Ok(instrument);
    }

    [HttpGet("{ticker}/prices/latest")]
    public async Task<ActionResult<MarketPriceDto>> GetLatestPrice(
        [FromRoute] string ticker)
    {
        var latestPrice = await _marketPriceService.GetLatestAsync(ticker);
        if (latestPrice is null)
        {
            return NotFound();
        }
        return Ok(latestPrice);
    }

    [HttpGet("{ticker}/prices/history")]
    public async Task<ActionResult<PagedResult<MarketPriceDto>>> GetPriceHistory(
        [FromRoute] string ticker,
        [FromQuery] PriceHistoryQuery query)
    {
        var history = await _marketPriceService.GetHistoryAsync(ticker, query);
        return Ok(history);
    }
}



