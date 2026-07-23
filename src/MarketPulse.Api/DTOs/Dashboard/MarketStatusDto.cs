namespace MarketPulse.Api.DTOs.Dashboard;

public class MarketStatusDto
{
    public string Country { get; set; } = "";
    public string Flag { get; set; } = "";
    public string Exchange { get; set; } = "";

    public bool IsOpen { get; set; }

    public string NextEvent { get; set; } = "";
}