using MarketPulse.Api.Data;
using MarketPulse.Api.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace MarketPulse.Api.Services
{
    public class DashboardOverviewService
    {
        private readonly AppDbContext _context;

        public DashboardOverviewService(AppDbContext context)
        {
            _context = context;
        }

    private async Task<int> GetTrackedInstrumentCount(CancellationToken cancellationToken)
    {
        return await _context.FinancialInstruments
            .CountAsync(i => i.IsActive, cancellationToken);
    }

    private async Task<int> GetMarketPriceCount(CancellationToken cancellationToken)
    {
        return await _context.MarketPrices
            .CountAsync(cancellationToken);
    }

    private async Task<DateTime?> GetLastImport(CancellationToken cancellationToken)
    {
        return await _context.MarketPrices
            .OrderByDescending(p => p.TimestampUtc)
            .Select(p => (DateTime?)p.TimestampUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<List<DashboardInstrument>> GetInstruments(CancellationToken cancellationToken)
    {
        return await _context.FinancialInstruments
            .Where(i => i.IsActive)
            .OrderBy(i => i.Ticker)
            .Select(i => new DashboardInstrument
            {
                Ticker = i.Ticker,
                Name = i.Name,
                Exchange = i.Exchange
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<DashboardOverviewDto> GetSummaryAsync(
            CancellationToken cancellationToken = default)
        {
            return new DashboardOverviewDto
            {
                TrackedInstruments =
                    await GetTrackedInstrumentCount(cancellationToken),

                MarketPrices =
                    await GetMarketPriceCount(cancellationToken),

                LastImportUtc =
                    await GetLastImport(cancellationToken),

                Instruments =
                    await GetInstruments(cancellationToken)
            };
        }
    }
}
