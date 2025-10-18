// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.IntegrationTests.Options;

/// <summary>
/// Integration tests for fetching previous day bar data for an options contract from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetPreviousDayBarIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching previous day bar data for an SPY call option.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_ForSPYCallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = "O:SPY251219C00650000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests the data types and structure of the previous day bar response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = "O:SPY251219C00650000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<PolygonResponse<List<Models.Options.OptionBar>>>(response);

        Assert.IsType<string>(response.Status);
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

        // Verify bar properties if results are present
        if (response.Results.Count > 0)
        {
            var bar = response.Results[0];

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
    /// Tests that the client correctly deserializes ticker and response metadata.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_ShouldDeserializeTickerAndMetadata()
    {
        // Arrange
        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = "O:SPY251219C00650000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify ticker and metadata are properly deserialized
        Assert.NotNull(response);
        Assert.Equal(request.OptionsTicker, response.Ticker);
        Assert.True(response.QueryCount >= 0, "QueryCount should be >= 0");
        Assert.True(response.ResultsCount >= 0, "ResultsCount should be >= 0");
    }

    /// <summary>
    /// Tests that the client correctly deserializes OHLC prices in the bar.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_ShouldDeserializeOHLCPrices()
    {
        // Arrange
        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = "O:SPY251219C00650000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify OHLC prices are properly deserialized
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            var bar = response.Results[0];

            // OHLC values should typically be present
            Assert.True(bar.Open.HasValue || bar.High.HasValue ||
                        bar.Low.HasValue || bar.Close.HasValue,
                "Bar should have at least one OHLC value");

            // If all OHLC values are present, verify relationships
            if (bar.Open.HasValue && bar.High.HasValue &&
                bar.Low.HasValue && bar.Close.HasValue)
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
    /// Tests that the client correctly deserializes volume and VWAP.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_ShouldDeserializeVolumeAndVWAP()
    {
        // Arrange
        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = "O:SPY251219C00650000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify volume and VWAP are properly deserialized
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            var bar = response.Results[0];

            if (bar.Volume.HasValue)
            {
                Assert.True(bar.Volume.Value >= 0,
                    $"Volume ({bar.Volume.Value}) should be >= 0");
            }

            if (bar.VolumeWeightedAveragePrice.HasValue)
            {
                Assert.True(bar.VolumeWeightedAveragePrice.Value >= 0,
                    $"VWAP ({bar.VolumeWeightedAveragePrice.Value}) should be >= 0");
            }
        }
    }

    /// <summary>
    /// Tests that the client correctly deserializes timestamp and transaction count.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_ShouldDeserializeTimestampAndTransactionCount()
    {
        // Arrange
        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = "O:SPY251219C00650000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify timestamp and transaction count are properly deserialized
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            var bar = response.Results[0];

            if (bar.Timestamp.HasValue)
            {
                Assert.True(bar.Timestamp.Value > 0,
                    $"Timestamp ({bar.Timestamp.Value}) should be > 0");
            }

            if (bar.NumberOfTransactions.HasValue)
            {
                Assert.True(bar.NumberOfTransactions.Value >= 0,
                    $"Number of transactions ({bar.NumberOfTransactions.Value}) should be >= 0");
            }
        }
    }

    /// <summary>
    /// Tests that the client correctly deserializes the adjusted parameter.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_WithAdjustedParameter_ShouldReturnValidResponse()
    {
        // Arrange
        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = "O:SPY251219C00650000",
            Adjusted = true
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call with the adjusted parameter
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }
}
