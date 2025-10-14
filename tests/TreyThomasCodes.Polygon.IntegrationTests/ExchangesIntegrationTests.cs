// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.Models.Common;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching exchanges data from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
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
    /// Tests fetching exchanges from Polygon.io with stocks asset class filter.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetExchanges_WithStocksFilter_ShouldReturnValidResponse()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: AssetClass.Stocks,
            locale: Locale.UnitedStates,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(exchangesResponse);
        Assert.Equal("OK", exchangesResponse.Status);
        Assert.NotEmpty(exchangesResponse.RequestId);
        Assert.True(exchangesResponse.Count > 0);
        Assert.NotNull(exchangesResponse.Results);
        Assert.True(exchangesResponse.Results.Count > 0);
    }

    /// <summary>
    /// Tests that the exchanges data structure is properly typed.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetExchanges_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: AssetClass.Stocks,
            locale: Locale.UnitedStates,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(exchangesResponse);
        Assert.IsType<List<Exchange>>(exchangesResponse.Results);
        Assert.IsType<int>(exchangesResponse.Count);
        Assert.IsType<string>(exchangesResponse.Status);
        Assert.IsType<string>(exchangesResponse.RequestId);

        // Verify at least one exchange has proper structure
        var firstExchange = exchangesResponse.Results.First();
        Assert.IsType<int>(firstExchange.Id);
        Assert.IsType<string>(firstExchange.Type);
        Assert.IsType<string>(firstExchange.AssetClass);
        Assert.IsType<string>(firstExchange.Locale);
        Assert.IsType<string>(firstExchange.Name);
        Assert.IsType<string>(firstExchange.OperatingMic);
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
