// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.RestClient.Validators.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services.Stocks;

/// <summary>
/// Unit tests for the simple parameter overload methods in StocksService.
/// These tests verify that the convenience overloads work correctly.
/// </summary>
public class StocksService_SimpleOverloadsTests
{
    [Fact]
    public async Task GetSnapshotAsync_WithTickerString_CallsRequestOverload()
    {
        // Arrange
        var mockApi = new Mock<IPolygonStocksApi>();
        var mockLogger = new Mock<ILogger<StocksService>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IValidator<GetSnapshotRequest>, GetSnapshotRequestValidator>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var expectedResponse = new StockSnapshotResponse
        {
            Status = "OK",
            Ticker = new StockSnapshot
            {
                Ticker = "AAPL",
                Day = new DayData { Open = 150.0m, Close = 151.0m }
            }
        };

        mockApi.Setup(x => x.GetSnapshotAsync("AAPL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var service = new StocksService(mockApi.Object, serviceProvider, mockLogger.Object);

        // Act - Using simple string overload
        var result = await service.GetSnapshotAsync("AAPL");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);
        Assert.Equal("AAPL", result.Ticker!.Ticker);

        mockApi.Verify(x => x.GetSnapshotAsync("AAPL", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLastTradeAsync_WithTickerString_CallsRequestOverload()
    {
        // Arrange
        var mockApi = new Mock<IPolygonStocksApi>();
        var mockLogger = new Mock<ILogger<StocksService>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IValidator<GetLastTradeRequest>, GetLastTradeRequestValidator>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var expectedResponse = new PolygonResponse<StockTrade>
        {
            Status = "OK",
            Results = new StockTrade
            {
                Price = 100.50m,
                Size = 100
            }
        };

        mockApi.Setup(x => x.GetLastTradeAsync("MSFT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var service = new StocksService(mockApi.Object, serviceProvider, mockLogger.Object);

        // Act - Using simple string overload
        var result = await service.GetLastTradeAsync("MSFT");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);
        Assert.Equal(100.50m, result.Results!.Price);

        mockApi.Verify(x => x.GetLastTradeAsync("MSFT", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLastQuoteAsync_WithTickerString_CallsRequestOverload()
    {
        // Arrange
        var mockApi = new Mock<IPolygonStocksApi>();
        var mockLogger = new Mock<ILogger<StocksService>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IValidator<GetLastQuoteRequest>, GetLastQuoteRequestValidator>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var expectedResponse = new PolygonResponse<LastQuoteResult>
        {
            Status = "OK",
            Results = new LastQuoteResult
            {
                AskPrice = 101.0m,
                BidPrice = 100.0m
            }
        };

        mockApi.Setup(x => x.GetLastQuoteAsync("TSLA", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var service = new StocksService(mockApi.Object, serviceProvider, mockLogger.Object);

        // Act - Using simple string overload
        var result = await service.GetLastQuoteAsync("TSLA");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);
        Assert.Equal(101.0m, result.Results!.AskPrice);
        Assert.Equal(100.0m, result.Results.BidPrice);

        mockApi.Verify(x => x.GetLastQuoteAsync("TSLA", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSnapshotAsync_WithEmptyTicker_ThrowsValidationException()
    {
        // Arrange
        var mockApi = new Mock<IPolygonStocksApi>();
        var mockLogger = new Mock<ILogger<StocksService>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IValidator<GetSnapshotRequest>, GetSnapshotRequestValidator>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var service = new StocksService(mockApi.Object, serviceProvider, mockLogger.Object);

        // Act & Assert - Empty ticker should fail validation
        await Assert.ThrowsAsync<Exceptions.PolygonValidationException>(
            async () => await service.GetSnapshotAsync(""));
    }
}
