using MarketPulse.Api.Data;
using MarketPulse.Api.Interfaces;
using MarketPulse.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketPulse.Api.Services;
public class HealthHistoryService : IHealthHistoryService
{
    private readonly AppDbContext _context;

    public HealthHistoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRecordAsync(string status, int responseTimeMs)
    {
        var record = new HealthCheckRecord
        {
            Status = status,
            ResponseTimeMs = responseTimeMs,
            CheckedAt = DateTime.UtcNow
        };

        _context.HealthChecks.Add(record);

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTotalChecksAsync()
    {
        return await _context.HealthChecks.CountAsync();
    }
}