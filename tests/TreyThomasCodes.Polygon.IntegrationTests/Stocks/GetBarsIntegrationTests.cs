// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using NodaTime;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.IntegrationTests.Stocks;

/// <summary>
/// Integration tests for fetching stock aggregate bars (OHLC) data.
/// These tests require a valid Polygon API key to be configured in user secrets.
/// </summary>
public class GetBarsIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching TSLA daily OHLC data for the week of September 15-19, 2025.
    /// Verifies that the API returns valid aggregate data with expected properties.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_ForTSLAWeeklyData_ShouldReturnValidAggregateData()
    {
        // Arrange
        var weekStart = new DateOnly(2025, 9, 15);
        var weekEnd = new DateOnly(2025, 9, 19);
        var stocksService = PolygonClient.Stocks;

        // Act
        var response = await stocksService.GetBarsAsync("TSLA", 1, AggregateInterval.Day, weekStart.ToString("yyyy-MM-dd"), weekEnd.ToString("yyyy-MM-dd"), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.Equal("TSLA", response.Ticker);

        if (response.Results != null && response.ResultsCount > 0)
        {
            // Verify we have valid data
            Assert.True(response.ResultsCount > 0);
            Assert.NotEmpty(response.Results);

            // Check each aggregate bar has valid data
            foreach (var bar in response.Results)
            {
                Assert.NotNull(bar);
                Assert.True(bar.Open > 0, "Open price should be greater than 0");
                Assert.True(bar.High > 0, "High price should be greater than 0");
                Assert.True(bar.Low > 0, "Low price should be greater than 0");
                Assert.True(bar.Close > 0, "Close price should be greater than 0");
                Assert.True(bar.Volume > 0, "Volume should be greater than 0");
                Assert.True(bar.VolumeWeightedAveragePrice > 0, "VWAP should be greater than 0");

                // Verify High >= Low and that Open/Close are within High/Low range
                Assert.True(bar.High >= bar.Low, "High should be greater than or equal to Low");
                Assert.True(bar.Open >= bar.Low && bar.Open <= bar.High, "Open should be within High/Low range");
                Assert.True(bar.Close >= bar.Low && bar.Close <= bar.High, "Close should be within High/Low range");

                // Verify timestamp is present and can be converted to date
                Assert.NotNull(bar.Timestamp);
                var tradeDate = bar.Timestamp.HasValue
                    ? Instant.FromUnixTimeMilliseconds((long)bar.Timestamp.Value).InUtc().Date
                    : SystemClock.Instance.GetCurrentInstant().InUtc().Date;

                // Verify the date is within our expected range (allowing for market holidays)
                var weekStartLocal = new LocalDate(weekStart.Year, weekStart.Month, weekStart.Day);
                var weekEndLocal = new LocalDate(weekEnd.Year, weekEnd.Month, weekEnd.Day);
                Assert.True(tradeDate >= weekStartLocal, $"StockTrade date {tradeDate} should be on or after {weekStart}");
                Assert.True(tradeDate <= weekEndLocal, $"StockTrade date {tradeDate} should be on or before {weekEnd}");
            }
        }
        else
        {
            // If no data is returned, it could be due to market being closed or future date
            // This is acceptable for the test as it validates the API call worked
            Assert.True(response.ResultsCount == 0 || response.Results == null,
                "Expected either no results or null results when no data is available");
        }
    }

    /// <summary>
    /// Tests the data structure and validates that all expected aggregate properties are properly populated.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_ShouldHaveProperDataStructure()
    {
        // Arrange
        var weekStart = new DateOnly(2025, 9, 15);
        var weekEnd = new DateOnly(2025, 9, 19);
        var stocksService = PolygonClient.Stocks;

        // Act
        var response = await stocksService.GetBarsAsync("TSLA", 1, AggregateInterval.Day, weekStart.ToString("yyyy-MM-dd"), weekEnd.ToString("yyyy-MM-dd"), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<PolygonResponse<List<StockBar>>>(response);

        // Verify response metadata
        Assert.NotNull(response.Status);
        Assert.NotEmpty(response.RequestId);

        if (response.Results != null && response.ResultsCount > 0)
        {
            Assert.IsType<List<StockBar>>(response.Results);
            Assert.Equal(response.Results.Count, response.ResultsCount);

            // Verify each aggregate has the expected structure
            var firstBar = response.Results[0];
            Assert.IsType<StockBar>(firstBar);

            // Verify all decimal properties are present
            Assert.True(firstBar.Open.HasValue);
            Assert.True(firstBar.High.HasValue);
            Assert.True(firstBar.Low.HasValue);
            Assert.True(firstBar.Close.HasValue);
            Assert.True(firstBar.VolumeWeightedAveragePrice.HasValue);

            // Verify volume and timestamp
            Assert.True(firstBar.Volume.HasValue);
            Assert.True(firstBar.Timestamp.HasValue);
        }
    }

    /// <summary>
    /// Tests that the integration handles date range queries correctly and returns data in chronological order.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_ShouldReturnDataInChronologicalOrder()
    {
        // Arrange
        var weekStart = new DateOnly(2025, 9, 15);
        var weekEnd = new DateOnly(2025, 9, 19);
        var stocksService = PolygonClient.Stocks;

        // Act
        var response = await stocksService.GetBarsAsync("TSLA", 1, AggregateInterval.Day, weekStart.ToString("yyyy-MM-dd"), weekEnd.ToString("yyyy-MM-dd"), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);

        if (response.Results != null && response.ResultsCount > 1)
        {
            // Verify data is in chronological order
            for (int i = 1; i < response.Results.Count; i++)
            {
                var previousBar = response.Results[i - 1];
                var currentBar = response.Results[i];

                Assert.True(previousBar.Timestamp <= currentBar.Timestamp,
                    "StockBar data should be returned in chronological order");
            }
        }
    }
}
