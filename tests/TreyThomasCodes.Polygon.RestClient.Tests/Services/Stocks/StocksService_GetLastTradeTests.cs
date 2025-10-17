// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the StocksService.GetLastTradeAsync method.
/// Tests the service's ability to retrieve last trade data from Polygon.io API.
/// </summary>
public class StocksService_GetLastTradeTests
{
    private readonly Mock<IPolygonStocksApi> _mockApi;
    private readonly StocksService _service;

    /// <summary>
    /// Initializes a new instance of the StocksService_GetLastTradeTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public StocksService_GetLastTradeTests()
    {
        _mockApi = new Mock<IPolygonStocksApi>();
        _service = new StocksService(_mockApi.Object);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync calls the API and returns the last trade response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_CallsApi_ReturnsLastTradeResponse()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Results = new StockTrade
            {
                Ticker = "AAPL",
                Conditions = new List<int> { 12, 37 },
                TimeframeStart = 1759264235449251800,
                Id = "200627",
                Price = 254.11m,
                Sequence = 9841657,
                TapeOrCorrection = 202,
                Size = 5,
                Timestamp = 1759264235449270800,
                Exchange = 4,
                ParticipantTimestamp = 1759264235447000000
            },
            Status = "OK",
            RequestId = "7453d68e76e95db17f29fbde6e5cce4d"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.Price, result.Results.Price);
        Assert.Equal(expectedResponse.Results.Size, result.Results.Size);
        Assert.Equal(expectedResponse.Results.Exchange, result.Results.Exchange);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.Id, result.Results.Id);
        Assert.Equal(expectedResponse.Results.Conditions, result.Results.Conditions);

        _mockApi.Verify(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_PassesCancellationToken()
    {
        // Arrange
        var ticker = "MSFT";
        var cancellationToken = new CancellationToken();
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Status = "OK",
            Results = new StockTrade { Ticker = ticker }
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(ticker, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetLastTradeAsync(ticker, cancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetLastTradeAsync(ticker, cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles different ticker symbols correctly.
    /// </summary>
    [Theory]
    [InlineData("AAPL")]
    [InlineData("MSFT")]
    [InlineData("TSLA")]
    [InlineData("GOOGL")]
    public async Task GetLastTradeAsync_WithDifferentTickers_CallsApiWithCorrectTicker(string ticker)
    {
        // Arrange
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Results = new StockTrade
            {
                Ticker = ticker,
                Price = 100.00m,
                Size = 50
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticker, result.Results?.Ticker);
        _mockApi.Verify(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var ticker = "INVALID";
        _mockApi.Setup(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Common.PolygonResponse<StockTrade>)null!);

        // Act
        var result = await _service.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Results);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles complete trade data with all fields populated.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithCompleteTradeData_ReturnsCompleteData()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Results = new StockTrade
            {
                Ticker = "AAPL",
                Timestamp = 1759264235449270800,
                TimeframeStart = 1759264235449251800,
                Sequence = 9841657,
                ParticipantTimestamp = 1759264235447000000,
                Conditions = new List<int> { 12, 37 },
                Id = "200627",
                Exchange = 4,
                Price = 254.11m,
                Size = 5,
                TapeOrCorrection = 202
            },
            Status = "OK",
            RequestId = "7453d68e76e95db17f29fbde6e5cce4d"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.TimeframeStart, result.Results.TimeframeStart);
        Assert.Equal(expectedResponse.Results.Sequence, result.Results.Sequence);
        Assert.Equal(expectedResponse.Results.ParticipantTimestamp, result.Results.ParticipantTimestamp);
        Assert.Equal(expectedResponse.Results.Conditions, result.Results.Conditions);
        Assert.Equal(expectedResponse.Results.Id, result.Results.Id);
        Assert.Equal(expectedResponse.Results.Exchange, result.Results.Exchange);
        Assert.Equal(expectedResponse.Results.Price, result.Results.Price);
        Assert.Equal(expectedResponse.Results.Size, result.Results.Size);
        Assert.Equal(expectedResponse.Results.TapeOrCorrection, result.Results.TapeOrCorrection);
    }
}
