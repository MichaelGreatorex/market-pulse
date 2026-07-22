namespace MarketPulse.Api.Configuration;

public class MarketDataOptions
{
    public const string SectionName = "MarketData";

    public int ImportIntervalMinutes { get; init; } = 60;
}