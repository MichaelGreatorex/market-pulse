using MarketPulse.Api.Models.Imports;
using MarketPulse.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketPulse.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly MarketPriceImportService _importService;

    public AdminController(
        MarketPriceImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("import-prices")]
    public async Task<ActionResult<ImportResult>> ImportPrices(
        CancellationToken cancellationToken)
    {
        var imported = await _importService
            .ImportLatestPricesAsync(cancellationToken);

        return Ok(imported);
    }
}