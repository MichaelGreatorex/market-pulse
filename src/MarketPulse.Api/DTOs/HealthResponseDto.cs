namespace MarketPulse.Api.DTOs;

public class HealthResponseDto
{
    public bool IsHealthy { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Database { get; set; } = string.Empty;

    public int ResponseTimeMs { get; set; }

    public DateTime CheckedAt { get; set; }

    public int TotalChecks { get; set; }
}