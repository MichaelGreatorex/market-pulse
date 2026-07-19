using MarketPulse.Api.Configuration;
using MarketPulse.Api.DTOs.Finnhub;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace MarketPulse.Api.Clients.Finnhub;

public class FinnhubMarketDataClient : IMarketDataClient
{
    private readonly HttpClient _httpClient;
    private readonly FinnhubOptions _options;
    private readonly ILogger<FinnhubMarketDataClient> _logger;

    public async Task<FinnhubQuoteResponse?> GetQuoteAsync(
        string ticker,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Requesting latest quote for {Ticker}",
            ticker);

        var response = await _httpClient.GetFromJsonAsync<FinnhubQuoteResponse>(
            $"quote?symbol={ticker}&token={_options.ApiKey}",
            cancellationToken);

        _logger.LogInformation(
            "Received latest quote for {Ticker}",
            ticker);

        return response;
    }

    public FinnhubMarketDataClient(
        HttpClient httpClient,
        IOptions<FinnhubOptions> options,
        ILogger<FinnhubMarketDataClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }   
        
}