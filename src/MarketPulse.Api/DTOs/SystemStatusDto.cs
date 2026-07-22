namespace MarketPulse.Api.Dtos;

public class SystemStatusDto
{
    public bool Healthy { get; set; }

    public DateTimeOffset? LastAttemptUtc { get; set; }

    public DateTimeOffset? LastSuccessfulRunUtc { get; set; }

    public string? LastError { get; set; }
}