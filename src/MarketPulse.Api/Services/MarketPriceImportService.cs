using MarketPulse.Api.Clients;
using MarketPulse.Api.Data;
using MarketPulse.Api.DTOs.Finnhub;
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
        
        var skipped = 0;

        var failed = 0;
        var errors = new List<ImportError>();

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

            FinnhubQuoteResponse? quote;

            try
            {
                quote = await _marketDataClient.GetQuoteAsync(
                    instrument.Ticker,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                failed++;

                errors.Add(new ImportError
                {
                    Ticker = instrument.Ticker,
                    Message = ex.Message
                });

                _logger.LogError(
                    ex,
                    "Failed importing price for {Ticker}",
                    instrument.Ticker);

                continue;
            }

            if (quote is null)
            {
                failed++;

                errors.Add(new ImportError
                {
                    Ticker = instrument.Ticker,
                    Message = "No quote returned from market data provider"
                });

                _logger.LogWarning(
                    "No quote returned for {Ticker}",
                    instrument.Ticker);

                continue;
            }

            var marketPrice = MarketPriceMapper.ToMarketPrice(
                quote,
                instrument.Id);

            var exists = await _context.MarketPrices.AnyAsync(
                p => p.FinancialInstrumentId == marketPrice.FinancialInstrumentId &&
                     p.TimestampUtc == marketPrice.TimestampUtc,
                cancellationToken);

            if (exists)
            {
                skipped++;

                _logger.LogInformation(
                    "Price for {Ticker} at {Timestamp} already exists, skipping",
                    instrument.Ticker,
                    marketPrice.TimestampUtc);  
                
                continue;
            }


            prices.Add(marketPrice);

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
            Skipped = skipped,
            Failed = failed,
            DurationMs = stopwatch.ElapsedMilliseconds,
            Errors = errors
        };
    }
}