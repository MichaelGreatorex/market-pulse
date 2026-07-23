using MarketPulse.Api.Dtos;

namespace MarketPulse.Api.Services.Monitoring;

public class SystemStatusService
{
    private readonly SystemStatusDto _status = new();

    public void RecordAttempt()
    {
        _status.LastAttemptUtc = DateTimeOffset.UtcNow;
    }

    public void RecordSuccess()
    {
        _status.Healthy = true;
        _status.LastSuccessfulRunUtc = DateTimeOffset.UtcNow;
        _status.LastError = null;
    }

    public void RecordFailure(Exception ex)
    {
        _status.Healthy = false;
        _status.LastError = ex.Message;
    }

    public SystemStatusDto GetStatus()
    {
        return new SystemStatusDto
        {
            Healthy = _status.Healthy,
            LastAttemptUtc = _status.LastAttemptUtc,
            LastSuccessfulRunUtc = _status.LastSuccessfulRunUtc,
            LastError = _status.LastError
        };
    }
}
