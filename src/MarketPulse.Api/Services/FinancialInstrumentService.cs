using System.Linq.Expressions;
using MarketPulse.Api.Data;
using MarketPulse.Api.DTOs;
using MarketPulse.Api.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<FinancialInstrumentDto>> GetAllAsync()
    {
        return await _context.FinancialInstruments
            .AsNoTracking()
            .OrderBy(i => i.Ticker)
            .Select(ToDto)
            .ToListAsync();
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