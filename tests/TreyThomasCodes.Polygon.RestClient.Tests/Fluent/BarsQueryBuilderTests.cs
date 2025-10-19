// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Fluent;
using TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Fluent;

/// <summary>
/// Unit tests for the BarsQueryBuilder fluent API.
/// </summary>
public class BarsQueryBuilderTests
{
    private readonly Mock<IStocksService> _mockService;

    public BarsQueryBuilderTests()
    {
        _mockService = new Mock<IStocksService>();
    }

    [Fact]
    public async Task ExecuteAsync_WithAllParameters_CallsServiceWithCorrectRequest()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockBar>>
        {
            Status = "OK",
            Results = new List<StockBar>
            {
                new StockBar { Open = 150.0m, High = 152.0m, Low = 149.0m, Close = 151.0m, Volume = 1000000 }
            }
        };

        _mockService
            .Setup(s => s.GetBarsAsync(It.IsAny<GetBarsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var builder = new BarsQueryBuilder(_mockService.Object, "AAPL");

        // Act
        var result = await builder
            .From("2025-09-01")
            .To("2025-09-30")
            .Daily()
            .Adjusted()
            .Descending()
            .Limit(50)
            .ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);
        Assert.Single(result.Results!);

        _mockService.Verify(
            s => s.GetBarsAsync(
                It.Is<GetBarsRequest>(r =>
                    r.Ticker == "AAPL" &&
                    r.From == "2025-09-01" &&
                    r.To == "2025-09-30" &&
                    r.Multiplier == 1 &&
                    r.Timespan == AggregateInterval.Day &&
                    r.Adjusted == true &&
                    r.Sort == SortOrder.Descending &&
                    r.Limit == 50),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UsingExtensionMethod_Works()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockBar>>
        {
            Status = "OK",
            Results = new List<StockBar>()
        };

        _mockService
            .Setup(s => s.GetBarsAsync(It.IsAny<GetBarsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act - Using fluent extension method
        var result = await _mockService.Object
            .Bars("MSFT")
            .From("2025-01-01")
            .To("2025-01-31")
            .Hourly(4)
            .ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);

        _mockService.Verify(
            s => s.GetBarsAsync(
                It.Is<GetBarsRequest>(r =>
                    r.Ticker == "MSFT" &&
                    r.Multiplier == 4 &&
                    r.Timespan == AggregateInterval.Hour),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithWeeklyInterval_SetsCorrectTimespan()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockBar>>
        {
            Status = "OK",
            Results = new List<StockBar>()
        };

        _mockService
            .Setup(s => s.GetBarsAsync(It.IsAny<GetBarsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _mockService.Object
            .Bars("TSLA")
            .From("2024-01-01")
            .To("2024-12-31")
            .Weekly()
            .ExecuteAsync();

        // Assert
        _mockService.Verify(
            s => s.GetBarsAsync(
                It.Is<GetBarsRequest>(r =>
                    r.Timespan == AggregateInterval.Week &&
                    r.Multiplier == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithForTicker_OverridesInitialTicker()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockBar>>
        {
            Status = "OK",
            Results = new List<StockBar>()
        };

        _mockService
            .Setup(s => s.GetBarsAsync(It.IsAny<GetBarsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act - Initialize with AAPL, then change to GOOG
        var result = await _mockService.Object
            .Bars("AAPL")
            .ForTicker("GOOG")
            .From("2025-01-01")
            .To("2025-01-31")
            .Daily()
            .ExecuteAsync();

        // Assert
        _mockService.Verify(
            s => s.GetBarsAsync(
                It.Is<GetBarsRequest>(r => r.Ticker == "GOOG"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutTicker_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new BarsQueryBuilder(_mockService.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await builder
                .From("2025-01-01")
                .To("2025-01-31")
                .Daily()
                .ExecuteAsync());

        Assert.Contains("Ticker is required", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutInterval_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new BarsQueryBuilder(_mockService.Object, "AAPL");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await builder
                .From("2025-01-01")
                .To("2025-01-31")
                .ExecuteAsync());

        Assert.Contains("Multiplier is required", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutFromDate_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new BarsQueryBuilder(_mockService.Object, "AAPL");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await builder
                .To("2025-01-31")
                .Daily()
                .ExecuteAsync());

        Assert.Contains("From date is required", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutToDate_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new BarsQueryBuilder(_mockService.Object, "AAPL");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await builder
                .From("2025-01-01")
                .Daily()
                .ExecuteAsync());

        Assert.Contains("To date is required", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithCustomInterval_SetsCorrectParameters()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockBar>>
        {
            Status = "OK",
            Results = new List<StockBar>()
        };

        _mockService
            .Setup(s => s.GetBarsAsync(It.IsAny<GetBarsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _mockService.Object
            .Bars("NVDA")
            .From("2025-01-01")
            .To("2025-01-31")
            .WithInterval(15, AggregateInterval.Minute)
            .ExecuteAsync();

        // Assert
        _mockService.Verify(
            s => s.GetBarsAsync(
                It.Is<GetBarsRequest>(r =>
                    r.Multiplier == 15 &&
                    r.Timespan == AggregateInterval.Minute),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithAscendingSort_SetsCorrectSortOrder()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<StockBar>>
        {
            Status = "OK",
            Results = new List<StockBar>()
        };

        _mockService
            .Setup(s => s.GetBarsAsync(It.IsAny<GetBarsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _mockService.Object
            .Bars("AMD")
            .From("2025-01-01")
            .To("2025-01-31")
            .Daily()
            .Ascending()
            .ExecuteAsync();

        // Assert
        _mockService.Verify(
            s => s.GetBarsAsync(
                It.Is<GetBarsRequest>(r => r.Sort == SortOrder.Ascending),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
