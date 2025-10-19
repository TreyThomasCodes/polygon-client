// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Fluent;
using TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Reference;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Fluent;

/// <summary>
/// Unit tests for the TickersQueryBuilder fluent API.
/// </summary>
public class TickersQueryBuilderTests
{
    private readonly Mock<IReferenceDataService> _mockService;

    public TickersQueryBuilderTests()
    {
        _mockService = new Mock<IReferenceDataService>();
    }

    [Fact]
    public async Task ExecuteAsync_WithSearchParameter_CallsServiceWithCorrectRequest()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockTicker>>
        {
            Status = "OK",
            Results = new List<StockTicker>
            {
                new StockTicker { Symbol = "AAPL", Name = "Apple Inc." },
                new StockTicker { Symbol = "MSFT", Name = "Microsoft Corporation" }
            }
        };

        _mockService
            .Setup(s => s.GetTickersAsync(It.IsAny<GetTickersRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var builder = new TickersQueryBuilder(_mockService.Object);

        // Act
        var result = await builder
            .Search("tech")
            .ActiveOnly()
            .Limit(100)
            .ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);
        Assert.Equal(2, result.Results!.Count);

        _mockService.Verify(
            s => s.GetTickersAsync(
                It.Is<GetTickersRequest>(r =>
                    r.Search == "tech" &&
                    r.Active == true &&
                    r.Limit == 100),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UsingExtensionMethod_Works()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockTicker>>
        {
            Status = "OK",
            Results = new List<StockTicker>()
        };

        _mockService
            .Setup(s => s.GetTickersAsync(It.IsAny<GetTickersRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act - Using fluent extension method
        var result = await _mockService.Object
            .Tickers()
            .OfType("ETF")
            .InMarket(Market.Stocks)
            .Ascending()
            .Limit(50)
            .ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);

        _mockService.Verify(
            s => s.GetTickersAsync(
                It.Is<GetTickersRequest>(r =>
                    r.Type == "ETF" &&
                    r.Market == Market.Stocks &&
                    r.Order == SortOrder.Ascending &&
                    r.Limit == 50),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithTickerRangeFilter_SetsCorrectParameters()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockTicker>>
        {
            Status = "OK",
            Results = new List<StockTicker>()
        };

        _mockService
            .Setup(s => s.GetTickersAsync(It.IsAny<GetTickersRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _mockService.Object
            .Tickers()
            .Between("A", "C")
            .ExecuteAsync();

        // Assert
        _mockService.Verify(
            s => s.GetTickersAsync(
                It.Is<GetTickersRequest>(r =>
                    r.TickerGte == "A" &&
                    r.TickerLte == "C"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithExchangeFilter_SetsCorrectParameters()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockTicker>>
        {
            Status = "OK",
            Results = new List<StockTicker>()
        };

        _mockService
            .Setup(s => s.GetTickersAsync(It.IsAny<GetTickersRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _mockService.Object
            .Tickers()
            .OnExchange("XNAS")
            .ExecuteAsync();

        // Assert
        _mockService.Verify(
            s => s.GetTickersAsync(
                It.Is<GetTickersRequest>(r => r.Exchange == "XNAS"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithActiveOnDate_SetsCorrectParameters()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockTicker>>
        {
            Status = "OK",
            Results = new List<StockTicker>()
        };

        _mockService
            .Setup(s => s.GetTickersAsync(It.IsAny<GetTickersRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _mockService.Object
            .Tickers()
            .ActiveOn("2025-01-15")
            .ExecuteAsync();

        // Assert
        _mockService.Verify(
            s => s.GetTickersAsync(
                It.Is<GetTickersRequest>(r => r.Date == "2025-01-15"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithInactiveOnly_SetsCorrectParameters()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockTicker>>
        {
            Status = "OK",
            Results = new List<StockTicker>()
        };

        _mockService
            .Setup(s => s.GetTickersAsync(It.IsAny<GetTickersRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _mockService.Object
            .Tickers()
            .InactiveOnly()
            .ExecuteAsync();

        // Assert
        _mockService.Verify(
            s => s.GetTickersAsync(
                It.Is<GetTickersRequest>(r => r.Active == false),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
