using MarketPulse.Api.Services;
using MarketPulse.Api.Configuration;
using Microsoft.Extensions.Options;
using MarketPulse.Api.Services.Monitoring;

namespace MarketPulse.Api.Services.Background;

public class MarketPriceImportWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MarketPriceImportWorker> _logger;
    private readonly TimeSpan _interval;
    private readonly SystemStatusService _systemStatus;

    public MarketPriceImportWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<MarketPriceImportWorker> logger,
        IOptions<MarketDataOptions> options,
        SystemStatusService systemStatus)

         
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _interval = TimeSpan.FromMinutes(options.Value.ImportIntervalMinutes);
        _systemStatus = systemStatus;
    }


    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Market price import worker started");


        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _systemStatus.RecordAttempt();

                using var scope = _scopeFactory.CreateScope();

                var importer = scope
                    .ServiceProvider
                    .GetRequiredService<MarketPriceImportService>();

                var result = await importer.ImportLatestPricesAsync(
                    stoppingToken);


                _logger.LogInformation(
                    "Market price import completed. Imported: {Imported}, Skipped (already up to date): {Skipped}, Failed: {Failed}",
                    result.Imported,
                    result.Skipped,
                    result.Failed);
                _logger.LogInformation(
                    "Import interval configured to {Minutes} minutes.",
                    _interval.TotalMinutes);

                _systemStatus.RecordSuccess();
            }
            catch (Exception ex)
            {
                _systemStatus.RecordFailure(ex);
                _logger.LogError(
                    ex,
                    "Market price import worker failed");
            }


            await Task.Delay(
                _interval,
                stoppingToken);
        }
    }
}