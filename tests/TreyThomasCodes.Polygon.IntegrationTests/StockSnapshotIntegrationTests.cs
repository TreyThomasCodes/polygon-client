// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching stock snapshot data.
/// These tests require a valid Polygon API key to be configured in user secrets.
/// </summary>
public class StockSnapshotIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the StockSnapshotIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public StockSnapshotIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<StockSnapshotIntegrationTests>();
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
    /// Tests fetching AAPL snapshot data and verifies the response structure and data validity.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ForAAPL_ShouldReturnValidSnapshotData()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);

        // Verify ticker data is present
        Assert.NotNull(response.Ticker);
        Assert.Equal(ticker, response.Ticker.Ticker);

        // Verify timestamp is present and recent (within last 7 days)
        Assert.NotNull(response.Ticker.Updated);
        var updatedTime = Instant.FromUnixTimeTicks(response.Ticker.Updated.Value / 100);
        var currentTime = SystemClock.Instance.GetCurrentInstant();
        var timeDifference = currentTime - updatedTime;
        Assert.True(timeDifference.TotalDays <= 7, $"Snapshot timestamp should be recent, but was {timeDifference.TotalDays} days old");

        // Verify day data if present
        if (response.Ticker.Day != null)
        {
            Assert.True(response.Ticker.Day.Open > 0, "Day open price should be greater than 0");
            Assert.True(response.Ticker.Day.High > 0, "Day high price should be greater than 0");
            Assert.True(response.Ticker.Day.Low > 0, "Day low price should be greater than 0");
            Assert.True(response.Ticker.Day.Close > 0, "Day close price should be greater than 0");
            Assert.True(response.Ticker.Day.Volume > 0, "Day volume should be greater than 0");
            Assert.True(response.Ticker.Day.VolumeWeightedAveragePrice > 0, "Day VWAP should be greater than 0");

            // Verify price relationships
            Assert.True(response.Ticker.Day.High >= response.Ticker.Day.Low, "High should be >= Low");
            Assert.True(response.Ticker.Day.Open >= response.Ticker.Day.Low && response.Ticker.Day.Open <= response.Ticker.Day.High, "Open should be within High/Low range");
            Assert.True(response.Ticker.Day.Close >= response.Ticker.Day.Low && response.Ticker.Day.Close <= response.Ticker.Day.High, "Close should be within High/Low range");
        }

        // Verify last quote data if present
        if (response.Ticker.LastQuote != null)
        {
            Assert.True(response.Ticker.LastQuote.BidPrice > 0, "Bid price should be greater than 0");
            Assert.True(response.Ticker.LastQuote.AskPrice > 0, "Ask price should be greater than 0");
            Assert.True(response.Ticker.LastQuote.BidSize > 0, "Bid size should be greater than 0");
            Assert.True(response.Ticker.LastQuote.AskSize > 0, "Ask size should be greater than 0");
            Assert.NotNull(response.Ticker.LastQuote.Timestamp);

            // Verify spread is reasonable (ask >= bid)
            Assert.True(response.Ticker.LastQuote.AskPrice >= response.Ticker.LastQuote.BidPrice, "Ask price should be >= Bid price");
        }

        // Verify last trade data if present
        if (response.Ticker.LastTrade != null)
        {
            Assert.True(response.Ticker.LastTrade.Price > 0, "Last trade price should be greater than 0");
            Assert.True(response.Ticker.LastTrade.Size > 0, "Last trade size should be greater than 0");
            Assert.NotNull(response.Ticker.LastTrade.Timestamp);
        }

        // Verify minute data if present
        if (response.Ticker.Min != null)
        {
            Assert.True(response.Ticker.Min.Open > 0, "Minute open price should be greater than 0");
            Assert.True(response.Ticker.Min.High > 0, "Minute high price should be greater than 0");
            Assert.True(response.Ticker.Min.Low > 0, "Minute low price should be greater than 0");
            Assert.True(response.Ticker.Min.Close > 0, "Minute close price should be greater than 0");
            Assert.True(response.Ticker.Min.Volume > 0, "Minute volume should be greater than 0");
            Assert.NotNull(response.Ticker.Min.Timestamp);

            // Verify minute price relationships
            Assert.True(response.Ticker.Min.High >= response.Ticker.Min.Low, "Minute high should be >= low");
            Assert.True(response.Ticker.Min.Open >= response.Ticker.Min.Low && response.Ticker.Min.Open <= response.Ticker.Min.High, "Minute open should be within high/low range");
            Assert.True(response.Ticker.Min.Close >= response.Ticker.Min.Low && response.Ticker.Min.Close <= response.Ticker.Min.High, "Minute close should be within high/low range");
        }

        // Verify previous day data if present
        if (response.Ticker.PrevDay != null)
        {
            Assert.True(response.Ticker.PrevDay.Open > 0, "Previous day open price should be greater than 0");
            Assert.True(response.Ticker.PrevDay.High > 0, "Previous day high price should be greater than 0");
            Assert.True(response.Ticker.PrevDay.Low > 0, "Previous day low price should be greater than 0");
            Assert.True(response.Ticker.PrevDay.Close > 0, "Previous day close price should be greater than 0");
            Assert.True(response.Ticker.PrevDay.Volume > 0, "Previous day volume should be greater than 0");

            // Verify previous day price relationships
            Assert.True(response.Ticker.PrevDay.High >= response.Ticker.PrevDay.Low, "Previous day high should be >= low");
            Assert.True(response.Ticker.PrevDay.Open >= response.Ticker.PrevDay.Low && response.Ticker.PrevDay.Open <= response.Ticker.PrevDay.High, "Previous day open should be within high/low range");
            Assert.True(response.Ticker.PrevDay.Close >= response.Ticker.PrevDay.Low && response.Ticker.PrevDay.Close <= response.Ticker.PrevDay.High, "Previous day close should be within high/low range");
        }
    }

    /// <summary>
    /// Tests fetching snapshots for multiple popular stock tickers.
    /// </summary>
    [Theory]
    [InlineData("AAPL")]
    [InlineData("MSFT")]
    [InlineData("GOOGL")]
    [InlineData("TSLA")]
    public async Task GetSnapshotAsync_ForPopularTickers_ShouldReturnValidData(string ticker)
    {
        // Arrange
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);

        Assert.NotNull(response.Ticker);
        Assert.Equal(ticker, response.Ticker.Ticker);

        // Basic validation that we got meaningful data
        Assert.NotNull(response.Ticker.Updated);
        Assert.True(response.Ticker.Updated > 0, "Updated timestamp should be positive");
    }

    /// <summary>
    /// Tests the data types and structure of the snapshot response.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<StockSnapshotResponse>(response);

        // Verify response structure
        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Ticker);
        Assert.IsType<StockSnapshot>(response.Ticker);
        Assert.IsType<string>(response.Ticker.Ticker);
        Assert.IsType<long>(response.Ticker.Updated);

        // Verify decimal types for price and percentage changes
        if (response.Ticker.TodaysChangePerc.HasValue)
            Assert.IsType<decimal>(response.Ticker.TodaysChangePerc.Value);

        if (response.Ticker.TodaysChange.HasValue)
            Assert.IsType<decimal>(response.Ticker.TodaysChange.Value);

        // Verify nested object types if present
        if (response.Ticker.Day != null)
        {
            Assert.IsType<DayData>(response.Ticker.Day);
            if (response.Ticker.Day.Open.HasValue)
                Assert.IsType<decimal>(response.Ticker.Day.Open.Value);
        }

        if (response.Ticker.LastQuote != null)
        {
            Assert.IsType<LastQuoteData>(response.Ticker.LastQuote);
            if (response.Ticker.LastQuote.BidPrice.HasValue)
                Assert.IsType<decimal>(response.Ticker.LastQuote.BidPrice.Value);
        }

        if (response.Ticker.LastTrade != null)
        {
            Assert.IsType<LastTradeData>(response.Ticker.LastTrade);
            if (response.Ticker.LastTrade.Price.HasValue)
                Assert.IsType<decimal>(response.Ticker.LastTrade.Price.Value);
        }

        if (response.Ticker.Min != null)
        {
            Assert.IsType<MinuteData>(response.Ticker.Min);
            if (response.Ticker.Min.TransactionCount.HasValue)
                Assert.IsType<int>(response.Ticker.Min.TransactionCount.Value);
        }

        if (response.Ticker.PrevDay != null)
        {
            Assert.IsType<PreviousDayData>(response.Ticker.PrevDay);
        }
    }

    /// <summary>
    /// Tests that the MarketUpdated property converts timestamps correctly to Eastern Time.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_MarketUpdated_ShouldConvertToEasternTime()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Ticker);
        Assert.NotNull(response.Ticker.Updated);

        // Verify MarketUpdated property conversion
        var marketUpdated = response.Ticker.MarketUpdated;
        Assert.NotNull(marketUpdated);

        // Verify it's in Eastern Time zone
        var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
        Assert.Equal(easternZone, marketUpdated.Value.Zone);

        // Verify the timestamp is reasonable (not in the future, not too far in the past)
        var currentTime = SystemClock.Instance.GetCurrentInstant().InZone(easternZone);
        Assert.True(marketUpdated.Value.ToInstant() <= currentTime.ToInstant(), "Market updated time should not be in the future");

        var timeDifference = currentTime.ToInstant() - marketUpdated.Value.ToInstant();
        Assert.True(timeDifference.TotalDays <= 7, "Market updated time should be recent (within 7 days)");
    }

    /// <summary>
    /// Tests that the MarketTimestamp properties in nested objects work correctly.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_NestedMarketTimestamps_ShouldConvertCorrectly()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Ticker);

        var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
        var currentTime = SystemClock.Instance.GetCurrentInstant().InZone(easternZone);

        // Verify LastQuote MarketTimestamp
        if (response.Ticker.LastQuote?.Timestamp != null)
        {
            var lastQuoteTime = response.Ticker.LastQuote.MarketTimestamp;
            Assert.NotNull(lastQuoteTime);
            Assert.Equal(easternZone, lastQuoteTime.Value.Zone);
            Assert.True(lastQuoteTime.Value.ToInstant() <= currentTime.ToInstant(), "Last quote time should not be in the future");
        }

        // Verify LastTrade MarketTimestamp
        if (response.Ticker.LastTrade?.Timestamp != null)
        {
            var lastTradeTime = response.Ticker.LastTrade.MarketTimestamp;
            Assert.NotNull(lastTradeTime);
            Assert.Equal(easternZone, lastTradeTime.Value.Zone);
            Assert.True(lastTradeTime.Value.ToInstant() <= currentTime.ToInstant(), "Last trade time should not be in the future");
        }

        // Verify Min MarketTimestamp
        if (response.Ticker.Min?.Timestamp != null)
        {
            var minTime = response.Ticker.Min.MarketTimestamp;
            Assert.NotNull(minTime);
            Assert.Equal(easternZone, minTime.Value.Zone);
            Assert.True(minTime.Value.ToInstant() <= currentTime.ToInstant(), "Minute bar time should not be in the future");
        }
    }

    /// <summary>
    /// Tests behavior when requesting snapshot for an invalid ticker symbol.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "INVALIDTICKER123";
        var stocksService = _polygonClient.Stocks;

        // Act & Assert
        // The Polygon API returns a 404 for invalid ticker symbols
        // This is expected behavior and should throw a Refit.ApiException
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => stocksService.GetSnapshotAsync(invalidTicker, TestContext.Current.CancellationToken));

        // Verify the exception details
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