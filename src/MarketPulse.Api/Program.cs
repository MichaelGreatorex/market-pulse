using MarketPulse.Api.Clients;
using MarketPulse.Api.Clients.Finnhub;
using MarketPulse.Api.Configuration;
using MarketPulse.Api.Data;
using MarketPulse.Api.HealthChecks;
using MarketPulse.Api.Interfaces;
using MarketPulse.Api.Services;
using MarketPulse.Api.Services.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddCheck<FinnhubHealthCheck>("finnhub");

builder.Services.AddScoped<FinancialInstrumentService>();

builder.Services.AddScoped<MarketPriceService>();

builder.Services.AddScoped<MarketPriceImportService>();

builder.Services.AddHostedService<MarketPriceImportWorker>();

builder.Services.AddScoped<DashboardService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



builder.Services.Configure<MarketDataOptions>(
    builder.Configuration.GetSection(MarketDataOptions.SectionName));

builder.Services.Configure<FinnhubOptions>(
    builder.Configuration.GetSection(FinnhubOptions.SectionName));

builder.Services.AddHttpClient<IMarketDataClient, FinnhubMarketDataClient>(
    (serviceProvider, client) =>
    {
        var options = serviceProvider
            .GetRequiredService<
                Microsoft.Extensions.Options.IOptions<FinnhubOptions>>()
            .Value;

        client.BaseAddress = new Uri(
            options.BaseUrl.TrimEnd('/') + "/");

        client.Timeout = TimeSpan.FromSeconds(10);
    });



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();



app.MapControllers();

app.MapHealthChecks("/health");

app.MapHealthChecks("/ready");

app.Run();
