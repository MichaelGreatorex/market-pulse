using MarketPulse.Api.Data;
using MarketPulse.Api.DTOs;
using MarketPulse.Api.Models.Pagination;
using MarketPulse.Api.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace MarketPulse.Api.Services;

public class MarketPriceService
{
    private readonly AppDbContext _context;

    public MarketPriceService(AppDbContext context)
    {
        _context = context;
    }

    // Retrieves the latest market price for a given financial instrument ticker.
    public async Task<MarketPriceDto?> GetLatestAsync(string ticker)
    {
        ticker = ticker.Trim().ToUpperInvariant();

        return await _context.MarketPrices
            .AsNoTracking()
            .Where(p => p.FinancialInstrument.Ticker == ticker)
            .OrderByDescending(p => p.TimestampUtc)
            .Select(p => new MarketPriceDto
            {
                TimestampUtc = p.TimestampUtc,
                ClosePrice = p.ClosePrice
            })
            .FirstOrDefaultAsync();
    }


    public async Task<PagedResult<MarketPriceDto>> GetHistoryAsync(
        string ticker,
        PriceHistoryQuery query)
    {
        ticker = ticker.Trim().ToUpperInvariant();

        var prices = _context.MarketPrices
            .AsNoTracking()
            .Where(p => p.FinancialInstrument.Ticker == ticker);

        if (query.From.HasValue)
        {
            var from = DateTime.SpecifyKind(query.From.Value, DateTimeKind.Utc);

            prices = prices.Where(p => p.TimestampUtc >= from);
        }

        if (query.To.HasValue)
        {
            var to = DateTime.SpecifyKind(query.To.Value, DateTimeKind.Utc);

            prices = prices.Where(p => p.TimestampUtc <= to);
        }

        var totalCount = await prices.CountAsync();

        var items = await prices
            .OrderByDescending(p => p.TimestampUtc)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(p => new MarketPriceDto
            {
                TimestampUtc = p.TimestampUtc,
                ClosePrice = p.ClosePrice
            })
            .ToListAsync();

        return new PagedResult<MarketPriceDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

    }



}