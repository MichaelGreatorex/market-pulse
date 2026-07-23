using MarketPulse.Api.Data;
using MarketPulse.Api.Dtos;
using MarketPulse.Api.DTOs.Dashboard;
using MarketPulse.Api.Services.Monitoring;
using Microsoft.EntityFrameworkCore;

namespace MarketPulse.Api.Services;

public class DashboardService
{
    private readonly AppDbContext _context;
    private readonly SystemStatusService _systemStatus;
    private readonly MarketSessionService _marketStatus;

    public DashboardService(AppDbContext context, SystemStatusService systemStatus, MarketSessionService marketStatus)
    {
        _context = context;
        _systemStatus = systemStatus;
        _marketStatus = marketStatus;
    }

    public async Task<DashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        var trackedInstruments = await _context.FinancialInstruments
            .CountAsync(i => i.IsActive, cancellationToken);

        var marketPrices = await _context.MarketPrices
            .CountAsync(cancellationToken);

        var lastImport = await _context.MarketPrices
            .OrderByDescending(p => p.TimestampUtc)
            .Select(p => (DateTime?)p.TimestampUtc)
            .FirstOrDefaultAsync(cancellationToken);

        var instruments = await _context.FinancialInstruments
            .Where(i => i.IsActive)
            .OrderBy(i => i.Ticker)
            .Select(i => new DashboardInstrument
            {
                Ticker = i.Ticker,
                Name = i.Name,
                Exchange = i.Exchange
            })
            .ToListAsync(cancellationToken);

        return new DashboardResponse
        {
            TrackedInstruments = await GetTrackedInstrumentCount(cancellationToken),
            MarketPrices = await GetMarketPriceCount(cancellationToken),
            LastImportUtc = await GetLastImport(cancellationToken),
            Instruments = await GetInstruments(cancellationToken),
            SystemStatus = _systemStatus.GetStatus(),
            MarketStatus = _marketStatus.GetStatus()
        };
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

}