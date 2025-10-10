using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Reference;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching market status data from Polygon.io.
/// These tests require a valid Polygon API key to be configured in user secrets.
/// </summary>
public class MarketStatusIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the MarketStatusIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public MarketStatusIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<MarketStatusIntegrationTests>();
        var apiKey = builder.Configuration["Polygon:ApiKey"];

        // Skip all tests in this class if no API key is configured
        Assert.SkipUnless(!string.IsNullOrEmpty(apiKey), "Polygon API key not configured in user secrets. Use: dotnet user-secrets set \"Polygon:ApiKey\" \"your-api-key-here\"");

        _apiKey = apiKey!; // Safe to use ! since we skip if null or empty

        builder.Services.AddPolygonClient(options =>
        {
            options.ApiKey = _apiKey;
        });

        _host = builder.Build();
        _polygonClient = _host.Services.GetRequiredService<IPolygonClient>();
    }

    /// <summary>
    /// Tests fetching current market status from Polygon.io.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetMarketStatus_ShouldReturnValidResponse()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(marketStatus);
        Assert.NotNull(marketStatus.Market);
        Assert.NotEmpty(marketStatus.Market);
        Assert.NotNull(marketStatus.ServerTime);
        Assert.NotEmpty(marketStatus.ServerTime);
    }

    /// <summary>
    /// Tests that the market status data structure is properly typed.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetMarketStatus_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(marketStatus);
        Assert.IsType<MarketStatus>(marketStatus);

        // Verify property types
        Assert.IsType<string>(marketStatus.Market);
        Assert.IsType<bool>(marketStatus.AfterHours);
        Assert.IsType<bool>(marketStatus.EarlyHours);
        Assert.IsType<string>(marketStatus.ServerTime);

        // Verify nested object types if present
        if (marketStatus.Exchanges != null)
        {
            Assert.IsType<ExchangesStatus>(marketStatus.Exchanges);
        }

        if (marketStatus.Currencies != null)
        {
            Assert.IsType<CurrencyMarketsStatus>(marketStatus.Currencies);
        }

        if (marketStatus.IndicesGroups != null)
        {
            Assert.IsType<IndicesGroupsStatus>(marketStatus.IndicesGroups);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _host.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}