using MarketPulse.Api.Data;
using MarketPulse.Api.Dtos;
using MarketPulse.Api.DTOs.Dashboard;
using MarketPulse.Api.Services.Monitoring;
using Microsoft.EntityFrameworkCore;

namespace MarketPulse.Api.Services;

public class DashboardService
{
    private readonly DashboardOverviewService _summary;
    private readonly SystemStatusService _systemStatus;
    private readonly MarketSessionService _marketStatus;

    public DashboardService(DashboardOverviewService summary, SystemStatusService systemStatus, MarketSessionService marketStatus)
    {
        _summary = summary;
        _systemStatus = systemStatus;
        _marketStatus = marketStatus;
    }

    public async Task<DashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        var summary = await _summary.GetSummaryAsync(cancellationToken);

        return new DashboardResponse
        {
            SystemStatus = _systemStatus.GetStatus(),
            MarketStatus = _marketStatus.GetStatus(),
            TrackedInstruments = summary.TrackedInstruments,
            MarketPrices = summary.MarketPrices,
            LastImportUtc = summary.LastImportUtc,
            Instruments = summary.Instruments
        };
    }           
}