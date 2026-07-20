using MarketPulse.Api.Data;
using Microsoft.EntityFrameworkCore;
using MarketPulse.Api.Interfaces;
using MarketPulse.Api.Services;
using MarketPulse.Api.Configuration;
using MarketPulse.Api.Clients;
using MarketPulse.Api.Clients.Finnhub;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<FinancialInstrumentService>();

builder.Services.AddScoped<MarketPriceService>();

builder.Services.AddScoped<MarketPriceImportService>();

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

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
