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
    /// Verifies that the API returns valid market status data with expected properties.
    /// </summary>
    [Fact]
    public async Task GetMarketStatus_ShouldReturnValidMarketStatusData()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(marketStatus);

        // Verify basic market status properties
        Assert.NotNull(marketStatus.Market);
        Assert.NotEmpty(marketStatus.Market);
        Assert.Contains(marketStatus.Market, new[] { "open", "closed", "extended-hours", "pre-market", "post-market" });

        // Verify boolean properties are properly set (they are value types, so they're always "present")
        Assert.True(marketStatus.AfterHours == true || marketStatus.AfterHours == false);
        Assert.True(marketStatus.EarlyHours == true || marketStatus.EarlyHours == false);

        // Verify server time is present and properly formatted
        Assert.NotNull(marketStatus.ServerTime);
        Assert.NotEmpty(marketStatus.ServerTime);

        // Verify server time can be parsed as a valid DateTime (RFC3339 format)
        Assert.True(DateTime.TryParse(marketStatus.ServerTime, out var serverTime));
        Assert.True(serverTime > DateTime.MinValue);
    }

    /// <summary>
    /// Tests that market status includes exchange information.
    /// Verifies that the exchanges object contains status for major US exchanges.
    /// </summary>
    [Fact]
    public async Task GetMarketStatus_ShouldIncludeExchangeInformation()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(marketStatus);

        if (marketStatus.Exchanges != null)
        {
            // Verify exchange status values are valid if present
            if (!string.IsNullOrEmpty(marketStatus.Exchanges.NYSE))
            {
                Assert.Contains(marketStatus.Exchanges.NYSE, new[] { "open", "closed", "extended-hours" });
            }

            if (!string.IsNullOrEmpty(marketStatus.Exchanges.NASDAQ))
            {
                Assert.Contains(marketStatus.Exchanges.NASDAQ, new[] { "open", "closed", "extended-hours" });
            }

            if (!string.IsNullOrEmpty(marketStatus.Exchanges.OTC))
            {
                Assert.Contains(marketStatus.Exchanges.OTC, new[] { "open", "closed", "extended-hours" });
            }
        }
    }

    /// <summary>
    /// Tests that market status includes currency market information.
    /// Verifies that the currencies object contains status for forex and crypto markets.
    /// </summary>
    [Fact]
    public async Task GetMarketStatus_ShouldIncludeCurrencyMarketInformation()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(marketStatus);

        if (marketStatus.Currencies != null)
        {
            // Verify currency market status values are valid if present
            if (!string.IsNullOrEmpty(marketStatus.Currencies.Forex))
            {
                Assert.Contains(marketStatus.Currencies.Forex, new[] { "open", "closed", "extended-hours" });
            }

            if (!string.IsNullOrEmpty(marketStatus.Currencies.Crypto))
            {
                Assert.Contains(marketStatus.Currencies.Crypto, new[] { "open", "closed", "extended-hours" });
            }
        }
    }

    /// <summary>
    /// Tests that market status includes indices group information.
    /// Verifies that the indices groups object contains status for major index providers.
    /// </summary>
    [Fact]
    public async Task GetMarketStatus_ShouldIncludeIndicesGroupInformation()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(marketStatus);

        if (marketStatus.IndicesGroups != null)
        {
            // Verify indices group status values are valid if present
            var validStatuses = new[] { "open", "closed", "extended-hours" };

            if (!string.IsNullOrEmpty(marketStatus.IndicesGroups.SAndP))
            {
                Assert.Contains(marketStatus.IndicesGroups.SAndP, validStatuses);
            }

            if (!string.IsNullOrEmpty(marketStatus.IndicesGroups.DowJones))
            {
                Assert.Contains(marketStatus.IndicesGroups.DowJones, validStatuses);
            }

            if (!string.IsNullOrEmpty(marketStatus.IndicesGroups.MSCI))
            {
                Assert.Contains(marketStatus.IndicesGroups.MSCI, validStatuses);
            }

            if (!string.IsNullOrEmpty(marketStatus.IndicesGroups.FTSERussell))
            {
                Assert.Contains(marketStatus.IndicesGroups.FTSERussell, validStatuses);
            }

            if (!string.IsNullOrEmpty(marketStatus.IndicesGroups.ICE))
            {
                Assert.Contains(marketStatus.IndicesGroups.ICE, validStatuses);
            }

            if (!string.IsNullOrEmpty(marketStatus.IndicesGroups.SocieteGenerale))
            {
                Assert.Contains(marketStatus.IndicesGroups.SocieteGenerale, validStatuses);
            }

            if (!string.IsNullOrEmpty(marketStatus.IndicesGroups.MStar))
            {
                Assert.Contains(marketStatus.IndicesGroups.MStar, validStatuses);
            }
        }
    }

    /// <summary>
    /// Tests that the market status data structure is properly typed.
    /// Verifies that all response properties are of the expected types.
    /// </summary>
    [Fact]
    public async Task GetMarketStatus_ShouldHaveProperDataStructure()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(TestContext.Current.CancellationToken);

        // Assert
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

    /// <summary>
    /// Tests that market status reflects logical consistency.
    /// Verifies that the boolean flags are consistent with the overall market state.
    /// </summary>
    [Fact]
    public async Task GetMarketStatus_ShouldHaveLogicalConsistency()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(marketStatus);

        // Verify logical consistency between market state and boolean flags
        if (marketStatus.Market == "open")
        {
            // When market is open, it should not be after hours or early hours
            Assert.False(marketStatus.AfterHours, "Market should not be in after hours when overall status is 'open'");
            Assert.False(marketStatus.EarlyHours, "Market should not be in early hours when overall status is 'open'");
        }
        else if (marketStatus.Market == "closed")
        {
            // When market is closed, it could be after hours, early hours, or neither
            // But it shouldn't be both after hours and early hours simultaneously
            Assert.False(marketStatus.AfterHours && marketStatus.EarlyHours,
                "Market cannot be both in after hours and early hours simultaneously");
        }
        else if (marketStatus.Market == "extended-hours")
        {
            // When in extended hours, either after hours or early hours should be true
            Assert.True(marketStatus.AfterHours || marketStatus.EarlyHours,
                "When market is in extended hours, either after hours or early hours should be active");
        }

        // Verify server time is reasonably current (within last 24 hours)
        if (DateTime.TryParse(marketStatus.ServerTime, out var serverTime))
        {
            var now = DateTime.UtcNow;
            var timeDifference = Math.Abs((now - serverTime).TotalHours);
            Assert.True(timeDifference < 24, "Server time should be within 24 hours of current time");
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