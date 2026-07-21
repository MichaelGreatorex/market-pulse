using MarketPulse.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarketPulseServices(builder.Configuration);

var app = builder.Build();

app.UseMarketPulse();

app.Run();