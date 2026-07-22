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

    public DashboardService(AppDbContext context, SystemStatusService systemStatus)
    {
        _context = context;
        _systemStatus = systemStatus;
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
            TrackedInstruments = trackedInstruments,
            MarketPrices = marketPrices,
            LastImportUtc = lastImport,
            Instruments = instruments,
            SystemStatus = new SystemStatusDto
            {
                Healthy = _systemStatus.Healthy,
                LastAttemptUtc = _systemStatus.LastAttemptUtc,
                LastSuccessfulRunUtc = _systemStatus.LastSuccessfulRunUtc,
                LastError = _systemStatus.LastError
            },
        };
    }
}