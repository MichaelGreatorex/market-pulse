namespace MarketPulse.Api.Interfaces;

public interface IHealthHistoryService
{
    Task AddRecordAsync(string status, int responseTimeMs);

    Task<int> GetTotalChecksAsync();
}