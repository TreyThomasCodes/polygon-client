// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching the last stock trade data.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class StockLastTradeIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the StockLastTradeIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public StockLastTradeIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<StockLastTradeIntegrationTests>();
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
    /// Tests fetching the last trade for AAPL.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForAAPL_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
        Assert.Equal(ticker, response.Results.Ticker);
    }

    /// <summary>
    /// Tests the data types and structure of the last trade response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<Models.Stocks.StockTrade>>(response);

        // Verify response structure
        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<Models.Stocks.StockTrade>(response.Results);
        Assert.IsType<string>(response.Results.Ticker);
        Assert.IsType<decimal>(response.Results.Price);
        Assert.IsType<long>(response.Results.Size);
        Assert.IsType<long>(response.Results.Timestamp);
        Assert.IsType<int>(response.Results.Exchange);
        Assert.IsType<string>(response.Results.Id);

        // Verify optional fields if present
        if (response.Results.Conditions != null)
            Assert.IsType<List<int>>(response.Results.Conditions);

        if (response.Results.TimeframeStart.HasValue)
            Assert.IsType<long>(response.Results.TimeframeStart.Value);

        if (response.Results.Sequence.HasValue)
            Assert.IsType<long>(response.Results.Sequence.Value);

        if (response.Results.ParticipantTimestamp.HasValue)
            Assert.IsType<long>(response.Results.ParticipantTimestamp.Value);

        if (response.Results.TapeOrCorrection.HasValue)
            Assert.IsType<int>(response.Results.TapeOrCorrection.Value);
    }

    /// <summary>
    /// Tests that the client correctly handles errors for invalid ticker symbols.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "INVALIDTICKER123";
        var stocksService = _polygonClient.Stocks;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => stocksService.GetLastTradeAsync(invalidTicker, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
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
