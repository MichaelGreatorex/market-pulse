namespace MarketPulse.Api.Services.Monitoring;

public class SystemStatusService
{
    public DateTimeOffset? LastAttemptUtc { get; private set; }

    public DateTimeOffset? LastSuccessfulRunUtc { get; private set; }

    public bool Healthy { get; private set; }

    public string? LastError { get; private set; }

    public void RecordAttempt()
    {
        LastAttemptUtc = DateTimeOffset.UtcNow;
    }

    public void RecordSuccess()
    {
        Healthy = true;
        LastSuccessfulRunUtc = DateTimeOffset.UtcNow;
        LastError = null;
    }

    public void RecordFailure(Exception exception)
    {
        Healthy = false;
        LastError = exception.Message;
    }
}