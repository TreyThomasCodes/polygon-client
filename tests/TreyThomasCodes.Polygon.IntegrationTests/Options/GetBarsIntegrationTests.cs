// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.IntegrationTests.Options;

/// <summary>
/// Integration tests for fetching aggregate bars (OHLC) for an options contract from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetBarsIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching daily bars for an SPY call option.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_ForSPYCallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var multiplier = 1;
        var timespan = Models.Common.AggregateInterval.Day;
        var from = "2023-01-09";
        var to = "2023-02-10";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests the data types and structure of the option bars response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var multiplier = 1;
        var timespan = Models.Common.AggregateInterval.Day;
        var from = "2023-01-09";
        var to = "2023-02-10";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<List<Models.Options.OptionBar>>>(response);

        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<List<Models.Options.OptionBar>>(response.Results);

        // Verify response metadata
        if (response.Ticker != null)
            Assert.IsType<string>(response.Ticker);

        if (response.QueryCount.HasValue)
            Assert.IsType<int>(response.QueryCount.Value);

        if (response.ResultsCount.HasValue)
            Assert.IsType<int>(response.ResultsCount.Value);

        if (response.Adjusted.HasValue)
            Assert.IsType<bool>(response.Adjusted.Value);

        if (response.Count.HasValue)
            Assert.IsType<int>(response.Count.Value);

        // If we have results, verify the first item structure
        if (response.Results.Count > 0)
        {
            var bar = response.Results[0];
            Assert.IsType<Models.Options.OptionBar>(bar);

            if (bar.Volume.HasValue)
                Assert.IsType<ulong>(bar.Volume.Value);

            if (bar.VolumeWeightedAveragePrice.HasValue)
                Assert.IsType<decimal>(bar.VolumeWeightedAveragePrice.Value);

            if (bar.Open.HasValue)
                Assert.IsType<decimal>(bar.Open.Value);

            if (bar.Close.HasValue)
                Assert.IsType<decimal>(bar.Close.Value);

            if (bar.High.HasValue)
                Assert.IsType<decimal>(bar.High.Value);

            if (bar.Low.HasValue)
                Assert.IsType<decimal>(bar.Low.Value);

            if (bar.Timestamp.HasValue)
                Assert.IsType<ulong>(bar.Timestamp.Value);

            if (bar.NumberOfTransactions.HasValue)
                Assert.IsType<int>(bar.NumberOfTransactions.Value);
        }
    }

    /// <summary>
    /// Tests that the client correctly deserializes ticker and metadata in the response.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_ShouldDeserializeTickerAndMetadata()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var multiplier = 1;
        var timespan = Models.Common.AggregateInterval.Day;
        var from = "2023-01-09";
        var to = "2023-02-10";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify metadata is properly deserialized
        Assert.NotNull(response);
        Assert.Equal(optionsTicker, response.Ticker);
        Assert.True(response.QueryCount.HasValue);
        Assert.True(response.ResultsCount.HasValue);
        Assert.True(response.Count.HasValue);
        Assert.True(response.Adjusted.HasValue);
    }

    /// <summary>
    /// Tests that the MarketTimestamp computed property converts timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_MarketTimestamp_ShouldConvertToEasternTime()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var multiplier = 1;
        var timespan = Models.Common.AggregateInterval.Day;
        var from = "2023-01-09";
        var to = "2023-02-10";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify MarketTimestamp conversion works
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0 && response.Results[0].Timestamp.HasValue)
        {
            Assert.NotNull(response.Results[0].MarketTimestamp);
            Assert.Equal("America/New_York", response.Results[0].MarketTimestamp.Value.Zone.Id);
        }
    }

    /// <summary>
    /// Tests that the client correctly deserializes OHLC prices.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_ShouldDeserializeOHLCPrices()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var multiplier = 1;
        var timespan = Models.Common.AggregateInterval.Day;
        var from = "2023-01-09";
        var to = "2023-02-10";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify OHLC prices are properly deserialized
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            var bar = response.Results[0];
            // OHLC values should typically be present
            Assert.True(bar.Open.HasValue || bar.High.HasValue || bar.Low.HasValue || bar.Close.HasValue,
                "Bar should have at least one OHLC value");

            // If all OHLC values are present, verify relationships
            if (bar.Open.HasValue && bar.High.HasValue && bar.Low.HasValue && bar.Close.HasValue)
            {
                Assert.True(bar.High.Value >= bar.Open.Value,
                    $"High ({bar.High.Value}) should be >= open ({bar.Open.Value})");
                Assert.True(bar.High.Value >= bar.Close.Value,
                    $"High ({bar.High.Value}) should be >= close ({bar.Close.Value})");
                Assert.True(bar.Low.Value <= bar.Open.Value,
                    $"Low ({bar.Low.Value}) should be <= open ({bar.Open.Value})");
                Assert.True(bar.Low.Value <= bar.Close.Value,
                    $"Low ({bar.Low.Value}) should be <= close ({bar.Close.Value})");
            }
        }
    }

    /// <summary>
    /// Tests that the client correctly handles different timespan intervals.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_WithMinuteInterval_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var multiplier = 5;
        var timespan = Models.Common.AggregateInterval.Minute;
        var from = "2023-01-09";
        var to = "2023-01-10";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
    }

    /// <summary>
    /// Tests that the client correctly handles optional parameters.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_WithOptionalParameters_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var multiplier = 1;
        var timespan = Models.Common.AggregateInterval.Day;
        var from = "2023-01-09";
        var to = "2023-02-10";
        var adjusted = true;
        var sort = Models.Common.SortOrder.Ascending;
        var limit = 10;
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, adjusted, sort, limit, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call with optional parameters
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotNull(response.Results);

        // Verify limit is respected (results count should be <= limit if data is available)
        if (response.Results.Count > 0)
        {
            Assert.True(response.Results.Count <= limit,
                $"Results count ({response.Results.Count}) should be <= limit ({limit})");
        }
    }

    /// <summary>
    /// Tests that the client correctly deserializes volume and transaction count.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_ShouldDeserializeVolumeAndTransactionCount()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var multiplier = 1;
        var timespan = Models.Common.AggregateInterval.Day;
        var from = "2023-01-09";
        var to = "2023-02-10";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify volume and transaction count are properly deserialized
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            var bar = response.Results[0];

            // Volume and number of transactions should typically be present and positive
            if (bar.Volume.HasValue)
            {
                Assert.True(bar.Volume.Value >= 0,
                    $"Volume ({bar.Volume.Value}) should be >= 0");
            }

            if (bar.NumberOfTransactions.HasValue)
            {
                Assert.True(bar.NumberOfTransactions.Value >= 0,
                    $"Number of transactions ({bar.NumberOfTransactions.Value}) should be >= 0");
            }

            // VWAP should be present if there's volume
            if (bar.Volume.HasValue && bar.Volume.Value > 0)
            {
                Assert.True(bar.VolumeWeightedAveragePrice.HasValue,
                    "VWAP should be present when volume > 0");
            }
        }
    }
}
