// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

namespace TreyThomasCodes.Polygon.IntegrationTests.Stocks;

/// <summary>
/// Integration tests for fetching the last stock trade data.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetLastTradeIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching the last trade for AAPL.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForAAPL_ShouldReturnValidResponse()
    {
        // Arrange
        var stocksService = PolygonClient.Stocks;

        var request = new GetLastTradeRequest
        {
            Ticker = "AAPL"
        };

        // Act
        var response = await stocksService.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
        Assert.Equal(request.Ticker, response.Results.Ticker);
    }

    /// <summary>
    /// Tests the data types and structure of the last trade response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var stocksService = PolygonClient.Stocks;

        var request = new GetLastTradeRequest
        {
            Ticker = "AAPL"
        };

        // Act
        var response = await stocksService.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

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
    /// Tests that the client correctly validates ticker symbols.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForInvalidTicker_ShouldThrowValidationException()
    {
        // Arrange
        var stocksService = PolygonClient.Stocks;

        var request = new GetLastTradeRequest
        {
            Ticker = "INVALIDTICKER123"
        };

        // Act & Assert - Verify validation catches invalid ticker (exceeds 10 character limit)
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => stocksService.GetLastTradeAsync(request, TestContext.Current.CancellationToken));

        Assert.Contains("Ticker", exception.Message);
    }
}
