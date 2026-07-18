namespace MarketPulse.Api.DTOs;

public class FinancialInstrumentDto
{
    public string Ticker { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Exchange { get; set; } = string.Empty;

    public string Currency { get; set; } = string.Empty;
}