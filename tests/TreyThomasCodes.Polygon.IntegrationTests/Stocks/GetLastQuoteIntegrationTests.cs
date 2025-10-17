// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.IntegrationTests.Stocks;

/// <summary>
/// Integration tests for fetching the last stock quote (NBBO) data.
/// These tests require a valid Polygon API key to be configured in user secrets.
/// </summary>
public class GetLastQuoteIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching the last quote for AAPL.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_ForAAPL_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = PolygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
        Assert.Equal(ticker, response.Results.Ticker);
    }

    /// <summary>
    /// Tests the data types and structure of the last quote response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = PolygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<Models.Stocks.LastQuoteResult>>(response);

        // Verify response structure
        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<Models.Stocks.LastQuoteResult>(response.Results);
        Assert.IsType<string>(response.Results.Ticker);
        Assert.IsType<decimal>(response.Results.BidPrice);
        Assert.IsType<decimal>(response.Results.AskPrice);
        Assert.IsType<long>(response.Results.BidSize);
        Assert.IsType<long>(response.Results.AskSize);
        Assert.IsType<long>(response.Results.Timestamp);
        Assert.IsType<int>(response.Results.BidExchange);
        Assert.IsType<int>(response.Results.AskExchange);

        // Verify optional fields if present
        if (response.Results.Indicators != null)
            Assert.IsType<List<int>>(response.Results.Indicators);

        if (response.Results.Sequence.HasValue)
            Assert.IsType<long>(response.Results.Sequence.Value);

        if (response.Results.ParticipantTimestamp.HasValue)
            Assert.IsType<long>(response.Results.ParticipantTimestamp.Value);

        if (response.Results.Tape.HasValue)
            Assert.IsType<int>(response.Results.Tape.Value);
    }

    /// <summary>
    /// Tests that the client correctly handles errors for invalid ticker symbols.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "INVALIDTICKER123";
        var stocksService = PolygonClient.Stocks;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => stocksService.GetLastQuoteAsync(invalidTicker, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }
}
