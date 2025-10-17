// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the StocksService.GetLastQuoteAsync method.
/// Tests the service's ability to retrieve last quote data from Polygon.io API.
/// </summary>
public class StocksService_GetLastQuoteTests
{
    private readonly Mock<IPolygonStocksApi> _mockApi;
    private readonly StocksService _service;

    /// <summary>
    /// Initializes a new instance of the StocksService_GetLastQuoteTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public StocksService_GetLastQuoteTests()
    {
        _mockApi = new Mock<IPolygonStocksApi>();
        _service = new StocksService(_mockApi.Object);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync calls the API and returns the last quote response.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_CallsApi_ReturnsLastQuoteResponse()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Results = new LastQuoteResult
            {
                Ticker = "AAPL",
                BidPrice = 254.05m,
                BidSize = 2,
                AskPrice = 254m,
                AskSize = 6,
                BidExchange = 12,
                AskExchange = 8,
                Tape = 3,
                Timestamp = 1759266523134155000L,
                ParticipantTimestamp = 1759266523134142700L,
                Sequence = 69066304L,
                Indicators = new List<int> { 604 }
            },
            Status = "OK",
            RequestId = "232e88d413f65da04b9bcd6063adbfcb"
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.BidPrice, result.Results.BidPrice);
        Assert.Equal(expectedResponse.Results.BidSize, result.Results.BidSize);
        Assert.Equal(expectedResponse.Results.AskPrice, result.Results.AskPrice);
        Assert.Equal(expectedResponse.Results.AskSize, result.Results.AskSize);
        Assert.Equal(expectedResponse.Results.BidExchange, result.Results.BidExchange);
        Assert.Equal(expectedResponse.Results.AskExchange, result.Results.AskExchange);
        Assert.Equal(expectedResponse.Results.Tape, result.Results.Tape);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.ParticipantTimestamp, result.Results.ParticipantTimestamp);
        Assert.Equal(expectedResponse.Results.Sequence, result.Results.Sequence);
        Assert.Equal(expectedResponse.Results.Indicators, result.Results.Indicators);

        _mockApi.Verify(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_PassesCancellationToken()
    {
        // Arrange
        var ticker = "MSFT";
        var cancellationToken = new CancellationToken();
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Status = "OK",
            Results = new LastQuoteResult { Ticker = ticker }
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(ticker, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetLastQuoteAsync(ticker, cancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetLastQuoteAsync(ticker, cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync handles different ticker symbols correctly.
    /// </summary>
    [Theory]
    [InlineData("AAPL")]
    [InlineData("MSFT")]
    [InlineData("TSLA")]
    [InlineData("GOOGL")]
    public async Task GetLastQuoteAsync_WithDifferentTickers_CallsApiWithCorrectTicker(string ticker)
    {
        // Arrange
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Results = new LastQuoteResult
            {
                Ticker = ticker,
                BidPrice = 100.00m,
                AskPrice = 100.10m
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticker, result.Results?.Ticker);
        _mockApi.Verify(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var ticker = "INVALID";
        _mockApi.Setup(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Common.PolygonResponse<LastQuoteResult>)null!);

        // Act
        var result = await _service.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Results);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync handles complete quote data with all fields populated.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_WithCompleteQuoteData_ReturnsCompleteData()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Results = new LastQuoteResult
            {
                Ticker = "AAPL",
                BidPrice = 254.05m,
                BidSize = 2,
                AskPrice = 254m,
                AskSize = 6,
                BidExchange = 12,
                AskExchange = 8,
                Tape = 3,
                Timestamp = 1759266523134155000L,
                ParticipantTimestamp = 1759266523134142700L,
                Sequence = 69066304L,
                Indicators = new List<int> { 604 }
            },
            Status = "OK",
            RequestId = "232e88d413f65da04b9bcd6063adbfcb"
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.BidPrice, result.Results.BidPrice);
        Assert.Equal(expectedResponse.Results.BidSize, result.Results.BidSize);
        Assert.Equal(expectedResponse.Results.AskPrice, result.Results.AskPrice);
        Assert.Equal(expectedResponse.Results.AskSize, result.Results.AskSize);
        Assert.Equal(expectedResponse.Results.BidExchange, result.Results.BidExchange);
        Assert.Equal(expectedResponse.Results.AskExchange, result.Results.AskExchange);
        Assert.Equal(expectedResponse.Results.Tape, result.Results.Tape);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.ParticipantTimestamp, result.Results.ParticipantTimestamp);
        Assert.Equal(expectedResponse.Results.Sequence, result.Results.Sequence);
        Assert.Equal(expectedResponse.Results.Indicators, result.Results.Indicators);
    }
}
