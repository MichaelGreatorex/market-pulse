using MarketPulse.Api.Dtos;

namespace MarketPulse.Api.DTOs.Dashboard;

public class DashboardResponse
{
    public int TrackedInstruments { get; init; }

    public int MarketPrices { get; init; }

    public DateTime? LastImportUtc { get; init; }

    public List<DashboardInstrument> Instruments { get; init; } = [];

    public SystemStatusDto SystemStatus { get; set; } = new();
}