// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching the last stock trade data.
/// These tests require a valid Polygon API key to be configured in user secrets.
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
    /// Tests fetching the last trade for AAPL and verifies the response structure and data validity.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForAAPL_ShouldReturnValidLastTradeData()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);

        // Verify trade data is present
        Assert.NotNull(response.Results);
        Assert.Equal(ticker, response.Results.Ticker);

        // Verify price and size are valid
        Assert.NotNull(response.Results.Price);
        Assert.True(response.Results.Price > 0, "Trade price should be greater than 0");
        Assert.NotNull(response.Results.Size);
        Assert.True(response.Results.Size > 0, "Trade size should be greater than 0");

        // Verify timestamp is present and recent (within last 7 days)
        Assert.NotNull(response.Results.Timestamp);
        var tradeTime = Instant.FromUnixTimeTicks(response.Results.Timestamp.Value / 100);
        var currentTime = SystemClock.Instance.GetCurrentInstant();
        var timeDifference = currentTime - tradeTime;
        Assert.True(timeDifference.TotalDays <= 7, $"Trade timestamp should be recent, but was {timeDifference.TotalDays} days old");

        // Verify exchange is present
        Assert.NotNull(response.Results.Exchange);
        Assert.True(response.Results.Exchange >= 0, "Exchange code should be non-negative");

        // Verify trade ID is present
        Assert.NotNull(response.Results.Id);
        Assert.NotEmpty(response.Results.Id);
    }

    /// <summary>
    /// Tests fetching the last trade for multiple popular stock tickers.
    /// </summary>
    [Theory]
    [InlineData("AAPL")]
    [InlineData("MSFT")]
    [InlineData("GOOGL")]
    [InlineData("TSLA")]
    public async Task GetLastTradeAsync_ForPopularTickers_ShouldReturnValidData(string ticker)
    {
        // Arrange
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.Equal(ticker, response.Results.Ticker);

        // Basic validation that we got meaningful data
        Assert.NotNull(response.Results.Price);
        Assert.True(response.Results.Price > 0, "Trade price should be greater than 0");
        Assert.NotNull(response.Results.Size);
        Assert.True(response.Results.Size > 0, "Trade size should be greater than 0");
        Assert.NotNull(response.Results.Timestamp);
        Assert.True(response.Results.Timestamp > 0, "Timestamp should be positive");
    }

    /// <summary>
    /// Tests the data types and structure of the last trade response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
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
    /// Tests that the MarketTimestamp property converts timestamps correctly to Eastern Time.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_MarketTimestamp_ShouldConvertToEasternTime()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);
        Assert.NotNull(response.Results.Timestamp);

        // Verify MarketTimestamp property conversion
        var marketTimestamp = response.Results.MarketTimestamp;
        Assert.NotNull(marketTimestamp);

        // Verify it's in Eastern Time zone
        var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
        Assert.Equal(easternZone, marketTimestamp.Value.Zone);

        // Verify the timestamp is reasonable (not in the future, not too far in the past)
        var currentTime = SystemClock.Instance.GetCurrentInstant().InZone(easternZone);
        Assert.True(marketTimestamp.Value.ToInstant() <= currentTime.ToInstant(), "Trade timestamp should not be in the future");

        var timeDifference = currentTime.ToInstant() - marketTimestamp.Value.ToInstant();
        Assert.True(timeDifference.TotalDays <= 7, "Trade timestamp should be recent (within 7 days)");
    }

    /// <summary>
    /// Tests that the MarketTimeframeStart property converts timestamps correctly when present.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_MarketTimeframeStart_ShouldConvertCorrectlyIfPresent()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        // Only verify if TimeframeStart is present
        if (response.Results.TimeframeStart.HasValue)
        {
            var marketTimeframeStart = response.Results.MarketTimeframeStart;
            Assert.NotNull(marketTimeframeStart);

            // Verify it's in Eastern Time zone
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            Assert.Equal(easternZone, marketTimeframeStart.Value.Zone);

            // Verify the timestamp is reasonable
            var currentTime = SystemClock.Instance.GetCurrentInstant().InZone(easternZone);
            Assert.True(marketTimeframeStart.Value.ToInstant() <= currentTime.ToInstant(), "Timeframe start should not be in the future");
        }
    }

    /// <summary>
    /// Tests that the MarketParticipantTimestamp property converts timestamps correctly when present.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_MarketParticipantTimestamp_ShouldConvertCorrectlyIfPresent()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        // Only verify if ParticipantTimestamp is present
        if (response.Results.ParticipantTimestamp.HasValue)
        {
            var marketParticipantTimestamp = response.Results.MarketParticipantTimestamp;
            Assert.NotNull(marketParticipantTimestamp);

            // Verify it's in Eastern Time zone
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            Assert.Equal(easternZone, marketParticipantTimestamp.Value.Zone);

            // Verify the timestamp is reasonable
            var currentTime = SystemClock.Instance.GetCurrentInstant().InZone(easternZone);
            Assert.True(marketParticipantTimestamp.Value.ToInstant() <= currentTime.ToInstant(), "Participant timestamp should not be in the future");
        }
    }

    /// <summary>
    /// Tests that condition codes are present and valid when returned.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ConditionCodes_ShouldBeValidIfPresent()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        // Only verify if Conditions are present
        if (response.Results.Conditions != null)
        {
            Assert.NotEmpty(response.Results.Conditions);

            // Verify all condition codes are non-negative
            foreach (var condition in response.Results.Conditions)
            {
                Assert.True(condition >= 0, $"Condition code {condition} should be non-negative");
            }
        }
    }

    /// <summary>
    /// Tests behavior when requesting the last trade for an invalid ticker symbol.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "INVALIDTICKER123";
        var stocksService = _polygonClient.Stocks;

        // Act & Assert
        // The Polygon API returns a 404 for invalid ticker symbols
        // This is expected behavior and should throw a Refit.ApiException
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => stocksService.GetLastTradeAsync(invalidTicker, TestContext.Current.CancellationToken));

        // Verify the exception details
        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }

    /// <summary>
    /// Tests that sequence number is valid when present.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_Sequence_ShouldBeValidIfPresent()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        // Only verify if Sequence is present
        if (response.Results.Sequence.HasValue)
        {
            Assert.True(response.Results.Sequence.Value >= 0, "Sequence number should be non-negative");
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
