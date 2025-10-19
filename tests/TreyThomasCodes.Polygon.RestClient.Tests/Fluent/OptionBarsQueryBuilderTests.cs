// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Fluent;
using TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Fluent;

/// <summary>
/// Unit tests for the OptionBarsQueryBuilder fluent API.
/// </summary>
public class OptionBarsQueryBuilderTests
{
    private readonly Mock<IOptionsService> _mockService;

    public OptionBarsQueryBuilderTests()
    {
        _mockService = new Mock<IOptionsService>();
    }

    [Fact]
    public async Task ExecuteAsync_WithAllParameters_CallsServiceWithCorrectRequest()
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<OptionBar>>
        {
            Status = "OK",
            Results = new List<OptionBar>
            {
                new OptionBar { Open = 150.0m, High = 152.0m, Low = 149.0m, Close = 151.0m, Volume = 1000000 }
            }
        };

        _mockService
            .Setup(s => s.GetBarsAsync(It.IsAny<GetBarsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var builder = new OptionBarsQueryBuilder(_mockService.Object, "O:SPY251219C00650000");

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
                    r.OptionsTicker == "O:SPY251219C00650000" &&
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
        var expectedResponse = new PolygonResponse<List<OptionBar>>
        {
            Status = "OK",
            Results = new List<OptionBar>()
        };

        _mockService
            .Setup(s => s.GetBarsAsync(It.IsAny<GetBarsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act - Using fluent extension method
        var result = await _mockService.Object
            .OptionBars("O:TSLA260320C00700000")
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
                    r.OptionsTicker == "O:TSLA260320C00700000" &&
                    r.Multiplier == 4 &&
                    r.Timespan == AggregateInterval.Hour),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutTicker_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new OptionBarsQueryBuilder(_mockService.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await builder
                .From("2025-01-01")
                .To("2025-01-31")
                .Daily()
                .ExecuteAsync());

        Assert.Contains("Options ticker is required", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutInterval_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new OptionBarsQueryBuilder(_mockService.Object, "O:SPY251219C00650000");

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
        var builder = new OptionBarsQueryBuilder(_mockService.Object, "O:SPY251219C00650000");

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
        var builder = new OptionBarsQueryBuilder(_mockService.Object, "O:SPY251219C00650000");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await builder
                .From("2025-01-01")
                .Daily()
                .ExecuteAsync());

        Assert.Contains("To date is required", exception.Message);
    }
}
