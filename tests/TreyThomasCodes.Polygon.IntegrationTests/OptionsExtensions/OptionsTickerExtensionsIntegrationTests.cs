// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Extensions;

namespace TreyThomasCodes.Polygon.IntegrationTests.OptionsExtensions;

/// <summary>
/// Integration tests for OptionsTicker-based extension methods in OptionsServiceExtensions.
/// These tests verify that extension methods accepting OptionsTicker objects properly
/// delegate to the underlying API calls.
/// </summary>
public class OptionsTickerExtensionsIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that GetContractDetailsAsync with OptionsTicker works when using parsed ticker.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithParsedTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var tickerString = "O:TSLA260320C00700000";
        var ticker = OptionsTicker.Parse(tickerString);
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetContractDetailsAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.Equal(tickerString, response.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync with OptionsTicker works when using OptionsTickerBuilder.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithBuilderPattern_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTickerBuilder()
            .WithUnderlying("SPY")
            .WithExpiration(2025, 12, 19)
            .AsCall()
            .WithStrike(650m)
            .BuildTicker();
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetContractDetailsAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.Equal("SPY", response.Results.UnderlyingTicker);
        Assert.Equal("call", response.Results.ContractType);
        Assert.Equal(650m, response.Results.StrikePrice);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync with OptionsTicker returns valid snapshot data.
    /// Note: Snapshot endpoint requires the option contract to be currently available in the market.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("SPY", new DateOnly(2025, 12, 19), OptionType.Call, 650m);
        var optionsService = PolygonClient.Options;

        try
        {
            // Act
            var response = await optionsService.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("OK", response.Status);
            Assert.NotNull(response.Results);
            Assert.NotNull(response.Results.Details);
            Assert.Contains("SPY", response.Results.Details.Ticker);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Contract may not be available for snapshot - skip test
            Assert.SkipWhen(true, "Option contract not available for snapshot in current market");
        }
    }

    /// <summary>
    /// Tests that GetLastTradeAsync with OptionsTicker returns valid trade data.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:TSLA260320C00700000");
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.Equal("O:TSLA260320C00700000", response.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetQuotesAsync with OptionsTicker returns valid quote data.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:SPY241220P00720000");
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(
            ticker,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests that GetQuotesAsync with OptionsTicker supports timestamp filtering.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithOptionsTickerAndTimestamp_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("SPY", new DateOnly(2024, 12, 20), OptionType.Put, 720m);
        var timestamp = "2024-12-01";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(
            ticker,
            timestamp: timestamp,
            limit: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
    }

    /// <summary>
    /// Tests that GetTradesAsync with OptionsTicker returns valid trade data.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("TSLA", new DateOnly(2021, 9, 3), OptionType.Call, 700m);
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(
            ticker,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests that GetTradesAsync with OptionsTicker supports timestamp filtering.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_WithOptionsTickerAndTimestamp_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:TSLA210903C00700000");
        var timestamp = "2021-09-03";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(
            ticker,
            timestamp: timestamp,
            limit: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
    }

    /// <summary>
    /// Tests that GetBarsAsync with OptionsTicker returns valid bar data.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTickerBuilder()
            .WithUnderlying("SPY")
            .WithExpiration(2025, 12, 19)
            .AsCall()
            .WithStrike(650m)
            .BuildTicker();
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(
            ticker,
            1,
            AggregateInterval.Day,
            new DateOnly(2023, 1, 9),
            new DateOnly(2023, 2, 10),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests that GetBarsAsync with OptionsTicker supports optional parameters.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_WithOptionsTickerAndOptionalParams_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("SPY", new DateOnly(2025, 12, 19), OptionType.Call, 650m);
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(
            ticker,
            1,
            AggregateInterval.Day,
            new DateOnly(2023, 1, 9),
            new DateOnly(2023, 2, 10),
            adjusted: true,
            sort: SortOrder.Ascending,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        // Verify limit is respected
        if (response.Results.Count > 0)
        {
            Assert.True(response.Results.Count <= 10);
        }
    }

    /// <summary>
    /// Tests that GetDailyOpenCloseAsync with OptionsTicker returns valid daily data.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:SPY251219C00650000");
        var date = "2023-01-09";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetDailyOpenCloseAsync(ticker, date, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.Equal("O:SPY251219C00650000", response.Symbol);
        Assert.Equal(date, response.From);
    }

    /// <summary>
    /// Tests that GetPreviousDayBarAsync with OptionsTicker returns valid previous day data.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("SPY", new DateOnly(2025, 12, 19), OptionType.Call, 650m);
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(ticker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.Equal("O:SPY251219C00650000", response.Ticker);
    }

    /// <summary>
    /// Tests that GetPreviousDayBarAsync with OptionsTicker supports the adjusted parameter.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_WithOptionsTickerAndAdjusted_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:SPY251219C00650000");
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(
            ticker,
            adjusted: true,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }
}
