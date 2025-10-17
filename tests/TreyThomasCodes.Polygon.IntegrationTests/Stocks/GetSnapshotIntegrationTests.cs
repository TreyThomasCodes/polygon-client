// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.IntegrationTests.Stocks;

/// <summary>
/// Integration tests for fetching stock snapshot data.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetSnapshotIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching AAPL snapshot data.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ForAAPL_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = PolygonClient.Stocks;

        // Act
        var response = await stocksService.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Ticker);
        Assert.Equal(ticker, response.Ticker.Ticker);
    }

    /// <summary>
    /// Tests the data types and structure of the snapshot response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = PolygonClient.Stocks;

        // Act
        var response = await stocksService.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<StockSnapshotResponse>(response);

        // Verify response structure
        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Ticker);
        Assert.IsType<StockSnapshot>(response.Ticker);
        Assert.IsType<string>(response.Ticker.Ticker);
        Assert.IsType<long>(response.Ticker.Updated);

        // Verify nested object types if present
        if (response.Ticker.Day != null)
            Assert.IsType<DayData>(response.Ticker.Day);

        if (response.Ticker.LastQuote != null)
            Assert.IsType<LastQuoteData>(response.Ticker.LastQuote);

        if (response.Ticker.LastTrade != null)
            Assert.IsType<LastTradeData>(response.Ticker.LastTrade);

        if (response.Ticker.Min != null)
            Assert.IsType<MinuteData>(response.Ticker.Min);

        if (response.Ticker.PrevDay != null)
            Assert.IsType<PreviousDayData>(response.Ticker.PrevDay);
    }

    /// <summary>
    /// Tests that the client correctly handles errors for invalid ticker symbols.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "INVALIDTICKER123";
        var stocksService = PolygonClient.Stocks;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => stocksService.GetSnapshotAsync(invalidTicker, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }
}
