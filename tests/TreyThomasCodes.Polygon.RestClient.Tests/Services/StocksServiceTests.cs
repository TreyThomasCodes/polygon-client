// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the StocksService class.
/// Tests the service's ability to retrieve stock market data from Polygon.io API.
/// </summary>
public class StocksServiceTests
{
    private readonly Mock<IPolygonStocksApi> _mockApi;
    private readonly StocksService _service;

    /// <summary>
    /// Initializes a new instance of the StocksServiceTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public StocksServiceTests()
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