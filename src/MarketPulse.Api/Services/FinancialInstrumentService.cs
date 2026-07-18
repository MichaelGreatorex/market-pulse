using MarketPulse.Api.Data;
using MarketPulse.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MarketPulse.Api.Services;

public class FinancialInstrumentService
{
    private readonly AppDbContext _context;

    public FinancialInstrumentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FinancialInstrumentDto>> GetAllAsync()
    {
        return await _context.FinancialInstruments
            .AsNoTracking()
            .OrderBy(i => i.Ticker)
            .Select(i => new FinancialInstrumentDto
            {
                Ticker = i.Ticker,
                Name = i.Name,
                Exchange = i.Exchange,
                Currency = i.Currency
            })
            .ToListAsync();
    }
}
