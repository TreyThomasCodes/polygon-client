// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.IntegrationTests.Options;

/// <summary>
/// Integration tests for fetching daily open/close data for an options contract from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetDailyOpenCloseIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching daily open/close data for an SPY call option.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_ForSPYCallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var date = "2023-01-09";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetDailyOpenCloseAsync(optionsTicker, date, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Symbol);
        Assert.NotNull(response.From);
    }

    /// <summary>
    /// Tests the data types and structure of the daily open/close response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var date = "2023-01-09";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetDailyOpenCloseAsync(optionsTicker, date, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Options.OptionDailyOpenClose>(response);

        Assert.IsType<string>(response.Status);

        // Verify properties
        if (response.From != null)
            Assert.IsType<string>(response.From);

        if (response.Symbol != null)
            Assert.IsType<string>(response.Symbol);

        if (response.Open.HasValue)
            Assert.IsType<decimal>(response.Open.Value);

        if (response.High.HasValue)
            Assert.IsType<decimal>(response.High.Value);

        if (response.Low.HasValue)
            Assert.IsType<decimal>(response.Low.Value);

        if (response.Close.HasValue)
            Assert.IsType<decimal>(response.Close.Value);

        if (response.Volume.HasValue)
            Assert.IsType<ulong>(response.Volume.Value);

        if (response.AfterHours.HasValue)
            Assert.IsType<decimal>(response.AfterHours.Value);

        if (response.PreMarket.HasValue)
            Assert.IsType<decimal>(response.PreMarket.Value);
    }

    /// <summary>
    /// Tests that the client correctly deserializes symbol and date properties.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_ShouldDeserializeSymbolAndDate()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var date = "2023-01-09";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetDailyOpenCloseAsync(optionsTicker, date, TestContext.Current.CancellationToken);

        // Assert - Verify symbol and date are properly deserialized
        Assert.NotNull(response);
        Assert.Equal("O:SPY251219C00650000", response.Symbol);
        Assert.Equal("2023-01-09", response.From);
    }

    /// <summary>
    /// Tests that the client correctly deserializes OHLC prices.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_ShouldDeserializeOHLCPrices()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var date = "2023-01-09";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetDailyOpenCloseAsync(optionsTicker, date, TestContext.Current.CancellationToken);

        // Assert - Verify OHLC prices are properly deserialized
        Assert.NotNull(response);

        // OHLC values should typically be present
        Assert.True(response.Open.HasValue || response.High.HasValue ||
                    response.Low.HasValue || response.Close.HasValue,
            "Daily data should have at least one OHLC value");

        // If all OHLC values are present, verify relationships
        if (response.Open.HasValue && response.High.HasValue &&
            response.Low.HasValue && response.Close.HasValue)
        {
            Assert.True(response.High.Value >= response.Open.Value,
                $"High ({response.High.Value}) should be >= open ({response.Open.Value})");
            Assert.True(response.High.Value >= response.Close.Value,
                $"High ({response.High.Value}) should be >= close ({response.Close.Value})");
            Assert.True(response.Low.Value <= response.Open.Value,
                $"Low ({response.Low.Value}) should be <= open ({response.Open.Value})");
            Assert.True(response.Low.Value <= response.Close.Value,
                $"Low ({response.Low.Value}) should be <= close ({response.Close.Value})");
        }
    }

    /// <summary>
    /// Tests that the client correctly deserializes pre-market and after-hours prices.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_ShouldDeserializePreMarketAndAfterHours()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var date = "2023-01-09";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetDailyOpenCloseAsync(optionsTicker, date, TestContext.Current.CancellationToken);

        // Assert - Verify pre-market and after-hours prices are present
        Assert.NotNull(response);

        // Pre-market and after-hours may or may not be present depending on trading activity
        // Just verify they deserialize correctly when present
        if (response.PreMarket.HasValue)
        {
            Assert.True(response.PreMarket.Value >= 0,
                $"PreMarket price ({response.PreMarket.Value}) should be >= 0");
        }

        if (response.AfterHours.HasValue)
        {
            Assert.True(response.AfterHours.Value >= 0,
                $"AfterHours price ({response.AfterHours.Value}) should be >= 0");
        }
    }

    /// <summary>
    /// Tests that the client correctly deserializes volume.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_ShouldDeserializeVolume()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var date = "2023-01-09";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetDailyOpenCloseAsync(optionsTicker, date, TestContext.Current.CancellationToken);

        // Assert - Verify volume is properly deserialized
        Assert.NotNull(response);

        if (response.Volume.HasValue)
        {
            Assert.True(response.Volume.Value >= 0,
                $"Volume ({response.Volume.Value}) should be >= 0");
        }
    }

    /// <summary>
    /// Tests that the client correctly handles errors for invalid dates.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_ForInvalidDate_ShouldThrowApiException()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var invalidDate = "9999-12-31"; // Future date with no data
        var optionsService = PolygonClient.Options;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => optionsService.GetDailyOpenCloseAsync(optionsTicker, invalidDate, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Contains("400", exception.Message);
    }

    /// <summary>
    /// Tests that the client correctly handles errors for invalid options tickers.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "O:INVALID000000C00000000";
        var date = "2023-01-09";
        var optionsService = PolygonClient.Options;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => optionsService.GetDailyOpenCloseAsync(invalidTicker, date, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }
}
