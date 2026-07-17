namespace MarketPulse.Api.Models;

public class HealthCheckRecord
{
    public int Id { get; set; }

    public string Status { get; set; } = string.Empty;

    public int ResponseTimeMs { get; set; }

    public DateTime CheckedAt { get; set; }
}
