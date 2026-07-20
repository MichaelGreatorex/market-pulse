using MarketPulse.Api.Clients;
using MarketPulse.Api.Data;
using MarketPulse.Api.Mappers;
using MarketPulse.Api.Models;
using MarketPulse.Api.Models.Imports;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MarketPulse.Api.Services;

public class MarketPriceImportService
{
    private readonly AppDbContext _context;
    private readonly IMarketDataClient _marketDataClient;
    private readonly ILogger<MarketPriceImportService> _logger;

    public MarketPriceImportService(
        AppDbContext context,
        IMarketDataClient marketDataClient,
        ILogger<MarketPriceImportService> logger)
    {
        _context = context;
        _marketDataClient = marketDataClient;
        _logger = logger;
    }

    public async Task<ImportResult> ImportLatestPricesAsync(
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        var instruments = await _context.FinancialInstruments
            .Where(i => i.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var prices = new List<MarketPrice>();

        foreach (var instrument in instruments)
        {
            _logger.LogInformation(
                "Importing latest price for {Ticker}",
                instrument.Ticker);

            var quote = await _marketDataClient.GetQuoteAsync(
                instrument.Ticker,
                cancellationToken);

            if (quote is null)
            {
                _logger.LogWarning(
                    "No quote returned for {Ticker}",
                    instrument.Ticker);

                continue;
            }

            prices.Add(
                MarketPriceMapper.ToMarketPrice(
                    quote,
                    instrument.Id));
        }

        _context.MarketPrices.AddRange(prices);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Imported {Count} prices",
            prices.Count);

        stopwatch.Stop();

        _logger.LogInformation(
            "Import completed in {Duration} ms",
            stopwatch.ElapsedMilliseconds);

        return new ImportResult
        {
            Imported = prices.Count,
            Skipped = 0,
            Failed = 0,
            DurationMs = stopwatch.ElapsedMilliseconds
        };
    }
}