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
/// Integration tests for fetching exchanges data from Polygon.io.
/// These tests require a valid Polygon API key to be configured in user secrets.
/// </summary>
public class ExchangesIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the ExchangesIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public ExchangesIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<ExchangesIntegrationTests>();
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
    /// Tests fetching exchanges from Polygon.io without any filters.
    /// Verifies that the API returns valid exchanges data matching the expected response structure.
    /// </summary>
    [Fact]
    public async Task GetExchanges_WithoutFilters_ShouldReturnValidExchangesData()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.Equal("OK", exchangesResponse.Status);
        Assert.NotEmpty(exchangesResponse.RequestId);
        Assert.True(exchangesResponse.Count > 0, "Should return at least one exchange");
        Assert.NotNull(exchangesResponse.Results);
        Assert.True(exchangesResponse.Results.Count > 0, "Results should contain at least one exchange");
    }

    /// <summary>
    /// Tests fetching exchanges from Polygon.io with stocks asset class filter.
    /// Verifies that the API returns valid exchanges data filtered for stocks.
    /// </summary>
    [Fact]
    public async Task GetExchanges_WithStocksFilter_ShouldReturnStockExchanges()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: "stocks",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.Equal("OK", exchangesResponse.Status);
        Assert.NotEmpty(exchangesResponse.RequestId);
        Assert.True(exchangesResponse.Count > 0, "Should return at least one stock exchange");
        Assert.NotNull(exchangesResponse.Results);
        Assert.True(exchangesResponse.Results.Count > 0, "Results should contain at least one exchange");

        // Verify all returned exchanges are for stocks
        foreach (var exchange in exchangesResponse.Results)
        {
            Assert.Equal("stocks", exchange.AssetClass);
        }
    }

    /// <summary>
    /// Tests fetching exchanges from Polygon.io with US locale filter.
    /// Verifies that the API returns valid exchanges data filtered for US locale.
    /// </summary>
    [Fact]
    public async Task GetExchanges_WithUsLocaleFilter_ShouldReturnUsExchanges()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            locale: "us",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.Equal("OK", exchangesResponse.Status);
        Assert.NotEmpty(exchangesResponse.RequestId);
        Assert.True(exchangesResponse.Count > 0, "Should return at least one US exchange");
        Assert.NotNull(exchangesResponse.Results);
        Assert.True(exchangesResponse.Results.Count > 0, "Results should contain at least one exchange");

        // Verify all returned exchanges are for US locale
        foreach (var exchange in exchangesResponse.Results)
        {
            Assert.Equal("us", exchange.Locale);
        }
    }

    /// <summary>
    /// Tests that exchanges include expected properties and data structure.
    /// Verifies that each exchange has all required fields populated with valid values.
    /// </summary>
    [Fact]
    public async Task GetExchanges_ShouldHaveValidExchangeStructure()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: "stocks",
            locale: "us",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.NotNull(exchangesResponse.Results);

        foreach (var exchange in exchangesResponse.Results.Take(5)) // Test first 5 to avoid too much output
        {
            // Verify required fields are present
            Assert.True(exchange.Id > 0, "ID should be a positive integer");

            Assert.NotNull(exchange.Type);
            Assert.NotEmpty(exchange.Type);

            Assert.NotNull(exchange.AssetClass);
            Assert.NotEmpty(exchange.AssetClass);

            Assert.NotNull(exchange.Locale);
            Assert.NotEmpty(exchange.Locale);

            Assert.NotNull(exchange.Name);
            Assert.NotEmpty(exchange.Name);

            Assert.NotNull(exchange.OperatingMic);
            Assert.NotEmpty(exchange.OperatingMic);

            // Verify the exchange object structure
            Assert.IsType<Exchange>(exchange);
        }
    }

    /// <summary>
    /// Tests that exchanges include specific expected exchanges.
    /// Verifies that common exchanges like NYSE, NASDAQ, etc. are present.
    /// </summary>
    [Fact]
    public async Task GetExchanges_ShouldIncludeExpectedExchanges()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;
        var expectedExchangeNames = new[] { "New York Stock Exchange", "Nasdaq", "NYSE American", "NYSE Arca" };

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: "stocks",
            locale: "us",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.NotNull(exchangesResponse.Results);

        var exchangeNames = exchangesResponse.Results.Select(e => e.Name).ToList();

        // At least some of the expected exchanges should be present
        var foundExpectedExchanges = expectedExchangeNames.Where(expected =>
            exchangeNames.Any(name => name.Contains(expected, StringComparison.OrdinalIgnoreCase))).Count();
        Assert.True(foundExpectedExchanges > 0,
            $"Should find at least one of the expected exchange names: {string.Join(", ", expectedExchangeNames)}. Found: {string.Join(", ", exchangeNames.Take(10))}");
    }

    /// <summary>
    /// Tests that exchanges have valid exchange types.
    /// Verifies that exchange types include expected values like "exchange", "TRF", "SIP", etc.
    /// </summary>
    [Fact]
    public async Task GetExchanges_ShouldHaveValidExchangeTypes()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;
        var expectedTypes = new[] { "exchange", "TRF", "SIP", "ORF" };

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: "stocks",
            locale: "us",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.NotNull(exchangesResponse.Results);

        var exchangeTypes = exchangesResponse.Results.Select(e => e.Type).Distinct().ToList();

        // At least some of the expected types should be present
        var foundExpectedTypes = expectedTypes.Where(expected => exchangeTypes.Contains(expected)).Count();
        Assert.True(foundExpectedTypes > 0,
            $"Should find at least one of the expected exchange types: {string.Join(", ", expectedTypes)}. Found: {string.Join(", ", exchangeTypes)}");
    }

    /// <summary>
    /// Tests that exchanges have valid MIC codes where present.
    /// Verifies that MIC codes are properly formatted when they exist.
    /// </summary>
    [Fact]
    public async Task GetExchanges_ShouldHaveValidMicCodes()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: "stocks",
            locale: "us",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.NotNull(exchangesResponse.Results);

        foreach (var exchange in exchangesResponse.Results.Take(10)) // Test first 10 to avoid too much output
        {
            // MIC codes are optional, but when present should be 4 characters
            if (!string.IsNullOrEmpty(exchange.Mic))
            {
                Assert.True(exchange.Mic.Length == 4,
                    $"MIC code should be 4 characters long, but got '{exchange.Mic}' for exchange '{exchange.Name}'");
                Assert.True(exchange.Mic.All(char.IsUpper),
                    $"MIC code should be uppercase, but got '{exchange.Mic}' for exchange '{exchange.Name}'");
            }

            // Operating MIC should always be present and be 4 characters
            Assert.NotNull(exchange.OperatingMic);
            Assert.True(exchange.OperatingMic.Length == 4,
                $"Operating MIC code should be 4 characters long, but got '{exchange.OperatingMic}' for exchange '{exchange.Name}'");
            Assert.True(exchange.OperatingMic.All(char.IsUpper),
                $"Operating MIC code should be uppercase, but got '{exchange.OperatingMic}' for exchange '{exchange.Name}'");
        }
    }

    /// <summary>
    /// Tests that exchanges have valid participant IDs where present.
    /// Verifies that participant IDs are single characters when they exist.
    /// </summary>
    [Fact]
    public async Task GetExchanges_ShouldHaveValidParticipantIds()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: "stocks",
            locale: "us",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.NotNull(exchangesResponse.Results);

        foreach (var exchange in exchangesResponse.Results.Take(10)) // Test first 10 to avoid too much output
        {
            // Participant IDs are optional, but when present should be single characters
            if (!string.IsNullOrEmpty(exchange.ParticipantId))
            {
                Assert.True(exchange.ParticipantId.Length == 1,
                    $"Participant ID should be a single character, but got '{exchange.ParticipantId}' for exchange '{exchange.Name}'");
                Assert.True(char.IsLetter(exchange.ParticipantId[0]),
                    $"Participant ID should be a letter, but got '{exchange.ParticipantId}' for exchange '{exchange.Name}'");
            }
        }
    }

    /// <summary>
    /// Tests that exchanges have valid URLs where present.
    /// Verifies that URLs are properly formatted when they exist.
    /// </summary>
    [Fact]
    public async Task GetExchanges_ShouldHaveValidUrls()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: "stocks",
            locale: "us",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.NotNull(exchangesResponse.Results);

        foreach (var exchange in exchangesResponse.Results.Take(10)) // Test first 10 to avoid too much output
        {
            // URLs are optional, but when present should be valid
            if (!string.IsNullOrEmpty(exchange.Url))
            {
                Assert.True(Uri.TryCreate(exchange.Url, UriKind.Absolute, out var uri),
                    $"URL should be valid, but got '{exchange.Url}' for exchange '{exchange.Name}'");
                Assert.True(uri.Scheme == "https" || uri.Scheme == "http",
                    $"URL should use HTTP or HTTPS scheme, but got '{uri.Scheme}' for exchange '{exchange.Name}'");
            }
        }
    }

    /// <summary>
    /// Tests that the exchanges data structure is properly typed.
    /// Verifies that all response properties are of the expected types.
    /// </summary>
    [Fact]
    public async Task GetExchanges_ShouldHaveProperDataStructure()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: "stocks",
            locale: "us",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.IsType<List<Exchange>>(exchangesResponse.Results);
        Assert.IsType<int>(exchangesResponse.Count);
        Assert.IsType<string>(exchangesResponse.Status);
        Assert.IsType<string>(exchangesResponse.RequestId);

        // Verify each exchange has proper structure
        foreach (var exchange in exchangesResponse.Results.Take(3)) // Test first 3 to avoid too much output
        {
            Assert.IsType<int>(exchange.Id);
            Assert.IsType<string>(exchange.Type);
            Assert.IsType<string>(exchange.AssetClass);
            Assert.IsType<string>(exchange.Locale);
            Assert.IsType<string>(exchange.Name);
            Assert.IsType<string>(exchange.OperatingMic);
            // Optional fields can be null, so we check if they're strings when not null
            Assert.True(exchange.Acronym == null || exchange.Acronym is string);
            Assert.True(exchange.Mic == null || exchange.Mic is string);
            Assert.True(exchange.ParticipantId == null || exchange.ParticipantId is string);
            Assert.True(exchange.Url == null || exchange.Url is string);
        }
    }

    /// <summary>
    /// Tests exchanges filtering with both asset class and locale filters.
    /// Verifies that multiple filters work together correctly.
    /// </summary>
    [Fact]
    public async Task GetExchanges_WithMultipleFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: "stocks",
            locale: "us",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(exchangesResponse);
        Assert.NotNull(exchangesResponse.Results);
        Assert.True(exchangesResponse.Results.Count > 0, "Should return at least one exchange");

        // Verify all returned exchanges match both filters
        foreach (var exchange in exchangesResponse.Results)
        {
            Assert.Equal("stocks", exchange.AssetClass);
            Assert.Equal("us", exchange.Locale);
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