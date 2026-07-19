using MarketPulse.Api.Data;
using MarketPulse.Api.DTOs;
using MarketPulse.Api.Models;
using MarketPulse.Api.Models.Pagination;
using MarketPulse.Api.Models.Queries;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MarketPulse.Api.Services;

public class FinancialInstrumentService
{
    private readonly AppDbContext _context;

    // Reusable projection from entity -> DTO.
    // Because this is an Expression, EF Core can translate it into SQL.
    private static readonly Expression<Func<FinancialInstrument, FinancialInstrumentDto>> ToDto =
        instrument => new FinancialInstrumentDto
        {
            Ticker = instrument.Ticker,
            Name = instrument.Name,
            Exchange = instrument.Exchange,
            Currency = instrument.Currency
        };

    public FinancialInstrumentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<FinancialInstrumentDto>> GetAllAsync(
        FinancialInstrumentQuery query)
    {
        // Start with the base query for financial instruments
        var instruments = _context.FinancialInstruments
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Exchange))
        {
            var exchange = query.Exchange.Trim().ToUpperInvariant();

            instruments = instruments.Where(i => i.Exchange == exchange);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            instruments = instruments.Where(i =>
                i.Name.Contains(search) ||
                i.Ticker.Contains(search));
        }

        // Get the total count before applying pagination
        var totalCount = await instruments.CountAsync();

        // Apply pagination and projection to DTOs
        var items = await instruments
            .OrderBy(i => i.Ticker)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(ToDto)
            .ToListAsync();

        return new PagedResult<FinancialInstrumentDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<FinancialInstrumentDto?> GetByTickerAsync(string ticker)
    {
        ticker = ticker.Trim().ToUpperInvariant();

        return await _context.FinancialInstruments
            .AsNoTracking()
            .Where(i => i.Ticker == ticker)
            .Select(ToDto)
            .FirstOrDefaultAsync();
    }
}