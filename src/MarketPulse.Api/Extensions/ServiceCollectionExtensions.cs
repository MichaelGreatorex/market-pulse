using MarketPulse.Api.Clients;
using MarketPulse.Api.Clients.Finnhub;
using MarketPulse.Api.Configuration;
using MarketPulse.Api.Data;
using MarketPulse.Api.HealthChecks;
using MarketPulse.Api.Services;
using MarketPulse.Api.Services.Background;
using MarketPulse.Api.Services.Monitoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MarketPulse.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMarketPulseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure PostgreSQL database context
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        // Configure health checks
        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("DefaultConnection")!)
            .AddCheck<FinnhubHealthCheck>("finnhub");

        // Register application services
        services.AddScoped<FinancialInstrumentService>();
        services.AddScoped<MarketPriceService>();
        services.AddScoped<MarketPriceImportService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<DashboardOverviewService>();

        // Register background worker for market price import
        services.AddHostedService<MarketPriceImportWorker>();

        // Configure options from appsettings.json
        services.Configure<MarketDataOptions>(
            configuration.GetSection(MarketDataOptions.SectionName));

        // Register system status service as a singleton
        services.AddSingleton<SystemStatusService>();

        // Register market status service as a singleton
        services.AddSingleton<MarketSessionService>();

        // Configure Finnhub options from appsettings.json
        services.Configure<FinnhubOptions>(
            configuration.GetSection(FinnhubOptions.SectionName));

        // Register Finnhub market data client with HttpClient
        services.AddHttpClient<IMarketDataClient, FinnhubMarketDataClient>(
            (serviceProvider, client) =>
            {
                var options = serviceProvider
                    .GetRequiredService<IOptions<FinnhubOptions>>()
                    .Value;

                client.BaseAddress = new Uri(
                    options.BaseUrl.TrimEnd('/') + "/");

                client.Timeout = TimeSpan.FromSeconds(10);
            });

        // Configure CORS to allow requests from the frontend application
        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // Add controllers and Swagger for API documentation
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}