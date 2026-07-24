namespace MarketPulse.Api.DTOs.Dashboard;

public class DashboardOverviewDto
{
    public int TrackedInstruments { get; set; }

    public int MarketPrices { get; set; }

    public DateTime? LastImportUtc { get; set; }

    public List<DashboardInstrument> Instruments { get; set; } = [];
}