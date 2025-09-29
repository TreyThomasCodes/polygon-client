// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TTC.Polygon.RestClient.Extensions;
using TTC.Polygon.RestClient.Services;
using TTC.Polygon.Models.Reference;

namespace TTC.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching ticker types data from Polygon.io.
/// These tests require a valid Polygon API key to be configured in user secrets.
/// </summary>
public class TickerTypesIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the TickerTypesIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public TickerTypesIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<TickerTypesIntegrationTests>();
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
    /// Tests fetching ticker types from Polygon.io.
    /// Verifies that the API returns valid ticker types data with expected properties.
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldReturnValidTickerTypesData()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(tickerTypesResponse);
        Assert.Equal("OK", tickerTypesResponse.Status);
        Assert.NotEmpty(tickerTypesResponse.RequestId);
        Assert.True(tickerTypesResponse.Count > 0, "Should return at least one ticker type");
        Assert.NotNull(tickerTypesResponse.Results);
        Assert.True(tickerTypesResponse.Results.Count > 0, "Results should contain at least one ticker type");
        Assert.Equal(tickerTypesResponse.Count, tickerTypesResponse.Results.Count);
    }

    /// <summary>
    /// Tests that ticker types include expected standard types.
    /// Verifies that common ticker types like CS, ETF, PFD are present in the response.
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldIncludeStandardTickerTypes()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;
        var expectedTickerTypes = new[] { "CS", "ETF", "PFD", "BOND", "WARRANT", "RIGHT" };

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(tickerTypesResponse);
        Assert.NotNull(tickerTypesResponse.Results);

        var tickerCodes = tickerTypesResponse.Results.Select(t => t.Code).ToList();

        foreach (var expectedType in expectedTickerTypes)
        {
            Assert.Contains(expectedType, tickerCodes);
        }
    }

    /// <summary>
    /// Tests that each ticker type has valid structure and data.
    /// Verifies that all ticker types have required fields populated with valid values.
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldHaveValidTickerTypeStructure()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(tickerTypesResponse);
        Assert.NotNull(tickerTypesResponse.Results);

        foreach (var tickerType in tickerTypesResponse.Results)
        {
            // Verify all required fields are present and not empty
            Assert.NotNull(tickerType.Code);
            Assert.NotEmpty(tickerType.Code);

            Assert.NotNull(tickerType.Description);
            Assert.NotEmpty(tickerType.Description);

            Assert.NotNull(tickerType.AssetClass);
            Assert.NotEmpty(tickerType.AssetClass);

            Assert.NotNull(tickerType.Locale);
            Assert.NotEmpty(tickerType.Locale);

            // Verify the ticker type object structure
            Assert.IsType<TickerType>(tickerType);
        }
    }

    /// <summary>
    /// Tests that ticker types include expected asset classes.
    /// Verifies that the response includes different asset classes like stocks, indices, etc.
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldIncludeMultipleAssetClasses()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(tickerTypesResponse);
        Assert.NotNull(tickerTypesResponse.Results);

        var assetClasses = tickerTypesResponse.Results.Select(t => t.AssetClass).Distinct().ToList();

        // Should include at least stocks asset class
        Assert.Contains("stocks", assetClasses);

        // Verify there's at least one asset class
        Assert.True(assetClasses.Count > 0, "Should have at least one asset class");
    }

    /// <summary>
    /// Tests that ticker types include US locale.
    /// Verifies that the response includes ticker types for the US market.
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldIncludeUSLocale()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(tickerTypesResponse);
        Assert.NotNull(tickerTypesResponse.Results);

        var locales = tickerTypesResponse.Results.Select(t => t.Locale).Distinct().ToList();

        // Should include US locale
        Assert.Contains("us", locales);
    }

    /// <summary>
    /// Tests specific ticker types have expected properties.
    /// Verifies that well-known ticker types have the expected descriptions and properties.
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldHaveExpectedSpecificTypes()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(tickerTypesResponse);
        Assert.NotNull(tickerTypesResponse.Results);

        // Verify Common Stock
        var commonStock = tickerTypesResponse.Results.FirstOrDefault(t => t.Code == "CS");
        if (commonStock != null)
        {
            Assert.Equal("Common Stock", commonStock.Description);
            Assert.Equal("stocks", commonStock.AssetClass);
            Assert.Equal("us", commonStock.Locale);
        }

        // Verify ETF
        var etf = tickerTypesResponse.Results.FirstOrDefault(t => t.Code == "ETF");
        if (etf != null)
        {
            Assert.Equal("Exchange Traded Fund", etf.Description);
            Assert.Equal("stocks", etf.AssetClass);
            Assert.Equal("us", etf.Locale);
        }

        // Verify Preferred Stock
        var preferredStock = tickerTypesResponse.Results.FirstOrDefault(t => t.Code == "PFD");
        if (preferredStock != null)
        {
            Assert.Equal("Preferred Stock", preferredStock.Description);
            Assert.Equal("stocks", preferredStock.AssetClass);
            Assert.Equal("us", preferredStock.Locale);
        }
    }

    /// <summary>
    /// Tests that the ticker types data structure is properly typed.
    /// Verifies that all response properties are of the expected types.
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldHaveProperDataStructure()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(tickerTypesResponse);
        Assert.IsType<TickerTypesResponse>(tickerTypesResponse);

        // Verify property types
        Assert.IsType<List<TickerType>>(tickerTypesResponse.Results);
        Assert.IsType<int>(tickerTypesResponse.Count);
        Assert.IsType<string>(tickerTypesResponse.Status);
        Assert.IsType<string>(tickerTypesResponse.RequestId);

        // Verify each ticker type has proper structure
        foreach (var tickerType in tickerTypesResponse.Results)
        {
            Assert.IsType<string>(tickerType.Code);
            Assert.IsType<string>(tickerType.Description);
            Assert.IsType<string>(tickerType.AssetClass);
            Assert.IsType<string>(tickerType.Locale);
        }
    }

    /// <summary>
    /// Tests that ticker types have reasonable count.
    /// Verifies that the API returns a reasonable number of ticker types (not empty, not excessively large).
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldHaveReasonableCount()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(tickerTypesResponse);

        // Should have a reasonable number of ticker types (at least 10, but not more than 1000)
        Assert.True(tickerTypesResponse.Count >= 10, "Should have at least 10 ticker types");
        Assert.True(tickerTypesResponse.Count <= 1000, "Should not have more than 1000 ticker types");

        // Count should match actual results
        Assert.Equal(tickerTypesResponse.Count, tickerTypesResponse.Results.Count);
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