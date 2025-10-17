// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the StocksService.GetSnapshotAsync method.
/// Tests the service's ability to retrieve stock snapshot data from Polygon.io API.
/// </summary>
public class StocksService_GetSnapshotTests
{
    private readonly Mock<IPolygonStocksApi> _mockApi;
    private readonly StocksService _service;

    /// <summary>
    /// Initializes a new instance of the StocksService_GetSnapshotTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public StocksService_GetSnapshotTests()
    {
        _mockApi = new Mock<IPolygonStocksApi>();
        _service = new StocksService(_mockApi.Object);
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when api parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenApiIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new StocksService(null!));
    }

    /// <summary>
    /// Tests that GetSnapshotAsync calls the API and returns the snapshot response.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_CallsApi_ReturnsSnapshotResponse()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new StockSnapshotResponse
        {
            Ticker = new StockSnapshot
            {
                Ticker = "AAPL",
                TodaysChangePerc = -0.537236734534977m,
                TodaysChange = -1.3799999999999955m,
                Updated = 1758931200000000000,
                Day = new DayData
                {
                    Open = 254.095m,
                    High = 257.6m,
                    Low = 253.78m,
                    Close = 255.46m,
                    Volume = 46293856,
                    VolumeWeightedAveragePrice = 255.4635m
                },
                LastQuote = new LastQuoteData
                {
                    BidPrice = 255.57m,
                    BidSize = 8,
                    AskPrice = 255.46m,
                    AskSize = 10,
                    Timestamp = 1758931135336996000
                },
                LastTrade = new LastTradeData
                {
                    Conditions = new List<int> { 12 },
                    Id = "217618",
                    Price = 255.49m,
                    Size = 100,
                    Timestamp = 1758931146328977200,
                    Exchange = 4
                },
                Min = new MinuteData
                {
                    AverageVolume = 46293856,
                    Timestamp = 1758931140000,
                    TransactionCount = 12,
                    Open = 255.5325m,
                    High = 255.5325m,
                    Low = 255.49m,
                    Close = 255.49m,
                    Volume = 322,
                    VolumeWeightedAveragePrice = 255.4987m
                },
                PrevDay = new PreviousDayData
                {
                    Open = 253.205m,
                    High = 257.17m,
                    Low = 251.712m,
                    Close = 256.87m,
                    Volume = 55202075,
                    VolumeWeightedAveragePrice = 254.8219m
                }
            },
            Status = "OK",
            RequestId = "5b2eb71330ca4cd2921f544fc3d4793d"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.NotNull(result.Ticker);
        Assert.Equal(expectedResponse.Ticker.Ticker, result.Ticker.Ticker);
        Assert.Equal(expectedResponse.Ticker.TodaysChangePerc, result.Ticker.TodaysChangePerc);
        Assert.Equal(expectedResponse.Ticker.TodaysChange, result.Ticker.TodaysChange);
        Assert.Equal(expectedResponse.Ticker.Updated, result.Ticker.Updated);

        _mockApi.Verify(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_PassesCancellationToken()
    {
        // Arrange
        var ticker = "MSFT";
        var cancellationToken = new CancellationToken();
        var expectedResponse = new StockSnapshotResponse { Status = "OK" };

        _mockApi.Setup(x => x.GetSnapshotAsync(ticker, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetSnapshotAsync(ticker, cancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetSnapshotAsync(ticker, cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles different ticker symbols correctly.
    /// </summary>
    [Theory]
    [InlineData("AAPL")]
    [InlineData("MSFT")]
    [InlineData("TSLA")]
    [InlineData("GOOGL")]
    public async Task GetSnapshotAsync_WithDifferentTickers_CallsApiWithCorrectTicker(string ticker)
    {
        // Arrange
        var expectedResponse = new StockSnapshotResponse
        {
            Ticker = new StockSnapshot { Ticker = ticker },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticker, result.Ticker?.Ticker);
        _mockApi.Verify(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var ticker = "INVALID";
        _mockApi.Setup(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockSnapshotResponse)null!);

        // Act
        var result = await _service.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles response with null ticker data.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WithNullTickerData_ReturnsResponseWithNullTicker()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new StockSnapshotResponse
        {
            Ticker = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Ticker);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles complex snapshot data with all nested objects populated.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WithComplexSnapshotData_ReturnsCompleteData()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new StockSnapshotResponse
        {
            Ticker = new StockSnapshot
            {
                Ticker = "AAPL",
                Value = 255.46m,
                TodaysChangePerc = -0.537236734534977m,
                TodaysChange = -1.3799999999999955m,
                Updated = 1758931200000000000,
                Day = new DayData
                {
                    Open = 254.095m,
                    High = 257.6m,
                    Low = 253.78m,
                    Close = 255.46m,
                    Volume = 46293856,
                    VolumeWeightedAveragePrice = 255.4635m
                },
                LastQuote = new LastQuoteData
                {
                    BidPrice = 255.57m,
                    BidSize = 8,
                    AskPrice = 255.46m,
                    AskSize = 10,
                    Timestamp = 1758931135336996000
                },
                LastTrade = new LastTradeData
                {
                    Conditions = new List<int> { 12 },
                    Id = "217618",
                    Price = 255.49m,
                    Size = 100,
                    Timestamp = 1758931146328977200,
                    Exchange = 4
                },
                Min = new MinuteData
                {
                    AverageVolume = 46293856,
                    Timestamp = 1758931140000,
                    TransactionCount = 12,
                    Open = 255.5325m,
                    High = 255.5325m,
                    Low = 255.49m,
                    Close = 255.49m,
                    Volume = 322,
                    VolumeWeightedAveragePrice = 255.4987m
                },
                PrevDay = new PreviousDayData
                {
                    Open = 253.205m,
                    High = 257.17m,
                    Low = 251.712m,
                    Close = 256.87m,
                    Volume = 55202075,
                    VolumeWeightedAveragePrice = 254.8219m
                }
            },
            Status = "OK",
            RequestId = "5b2eb71330ca4cd2921f544fc3d4793d"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Ticker);
        Assert.Equal(expectedResponse.Ticker.Ticker, result.Ticker.Ticker);
        Assert.Equal(expectedResponse.Ticker.Value, result.Ticker.Value);
        Assert.Equal(expectedResponse.Ticker.TodaysChangePerc, result.Ticker.TodaysChangePerc);
        Assert.Equal(expectedResponse.Ticker.TodaysChange, result.Ticker.TodaysChange);
        Assert.Equal(expectedResponse.Ticker.Updated, result.Ticker.Updated);

        // Verify Day data
        Assert.NotNull(result.Ticker.Day);
        Assert.Equal(expectedResponse.Ticker.Day.Open, result.Ticker.Day.Open);
        Assert.Equal(expectedResponse.Ticker.Day.High, result.Ticker.Day.High);
        Assert.Equal(expectedResponse.Ticker.Day.Low, result.Ticker.Day.Low);
        Assert.Equal(expectedResponse.Ticker.Day.Close, result.Ticker.Day.Close);
        Assert.Equal(expectedResponse.Ticker.Day.Volume, result.Ticker.Day.Volume);
        Assert.Equal(expectedResponse.Ticker.Day.VolumeWeightedAveragePrice, result.Ticker.Day.VolumeWeightedAveragePrice);

        // Verify LastQuote data
        Assert.NotNull(result.Ticker.LastQuote);
        Assert.Equal(expectedResponse.Ticker.LastQuote.BidPrice, result.Ticker.LastQuote.BidPrice);
        Assert.Equal(expectedResponse.Ticker.LastQuote.BidSize, result.Ticker.LastQuote.BidSize);
        Assert.Equal(expectedResponse.Ticker.LastQuote.AskPrice, result.Ticker.LastQuote.AskPrice);
        Assert.Equal(expectedResponse.Ticker.LastQuote.AskSize, result.Ticker.LastQuote.AskSize);
        Assert.Equal(expectedResponse.Ticker.LastQuote.Timestamp, result.Ticker.LastQuote.Timestamp);

        // Verify LastTrade data
        Assert.NotNull(result.Ticker.LastTrade);
        Assert.Equal(expectedResponse.Ticker.LastTrade.Conditions, result.Ticker.LastTrade.Conditions);
        Assert.Equal(expectedResponse.Ticker.LastTrade.Id, result.Ticker.LastTrade.Id);
        Assert.Equal(expectedResponse.Ticker.LastTrade.Price, result.Ticker.LastTrade.Price);
        Assert.Equal(expectedResponse.Ticker.LastTrade.Size, result.Ticker.LastTrade.Size);
        Assert.Equal(expectedResponse.Ticker.LastTrade.Timestamp, result.Ticker.LastTrade.Timestamp);
        Assert.Equal(expectedResponse.Ticker.LastTrade.Exchange, result.Ticker.LastTrade.Exchange);

        // Verify Min data
        Assert.NotNull(result.Ticker.Min);
        Assert.Equal(expectedResponse.Ticker.Min.AverageVolume, result.Ticker.Min.AverageVolume);
        Assert.Equal(expectedResponse.Ticker.Min.Timestamp, result.Ticker.Min.Timestamp);
        Assert.Equal(expectedResponse.Ticker.Min.TransactionCount, result.Ticker.Min.TransactionCount);
        Assert.Equal(expectedResponse.Ticker.Min.Open, result.Ticker.Min.Open);
        Assert.Equal(expectedResponse.Ticker.Min.High, result.Ticker.Min.High);
        Assert.Equal(expectedResponse.Ticker.Min.Low, result.Ticker.Min.Low);
        Assert.Equal(expectedResponse.Ticker.Min.Close, result.Ticker.Min.Close);
        Assert.Equal(expectedResponse.Ticker.Min.Volume, result.Ticker.Min.Volume);
        Assert.Equal(expectedResponse.Ticker.Min.VolumeWeightedAveragePrice, result.Ticker.Min.VolumeWeightedAveragePrice);

        // Verify PrevDay data
        Assert.NotNull(result.Ticker.PrevDay);
        Assert.Equal(expectedResponse.Ticker.PrevDay.Open, result.Ticker.PrevDay.Open);
        Assert.Equal(expectedResponse.Ticker.PrevDay.High, result.Ticker.PrevDay.High);
        Assert.Equal(expectedResponse.Ticker.PrevDay.Low, result.Ticker.PrevDay.Low);
        Assert.Equal(expectedResponse.Ticker.PrevDay.Close, result.Ticker.PrevDay.Close);
        Assert.Equal(expectedResponse.Ticker.PrevDay.Volume, result.Ticker.PrevDay.Volume);
        Assert.Equal(expectedResponse.Ticker.PrevDay.VolumeWeightedAveragePrice, result.Ticker.PrevDay.VolumeWeightedAveragePrice);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles responses with minimal data correctly.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WithMinimalData_ReturnsPartialSnapshot()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResponse = new StockSnapshotResponse
        {
            Ticker = new StockSnapshot
            {
                Ticker = "AAPL",
                Updated = 1758931200000000000
                // Only basic data, nested objects are null
            },
            Status = "OK",
            RequestId = "minimal-data-request"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Ticker);
        Assert.Equal(expectedResponse.Ticker.Ticker, result.Ticker.Ticker);
        Assert.Equal(expectedResponse.Ticker.Updated, result.Ticker.Updated);

        // Verify that nested objects can be null without issues
        Assert.Null(result.Ticker.Day);
        Assert.Null(result.Ticker.LastQuote);
        Assert.Null(result.Ticker.LastTrade);
        Assert.Null(result.Ticker.Min);
        Assert.Null(result.Ticker.PrevDay);
    }
}
