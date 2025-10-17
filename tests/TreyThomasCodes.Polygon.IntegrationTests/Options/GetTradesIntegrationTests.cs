// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.IntegrationTests.Options;

/// <summary>
/// Integration tests for fetching trades for an options contract from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetTradesIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching trades for a TSLA call option.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_ForTSLACallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:TSLA210903C00700000";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(optionsTicker, limit: 10, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests the data types and structure of the option trades response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var optionsTicker = "O:TSLA210903C00700000";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(optionsTicker, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<List<Models.Options.OptionTradeV3>>>(response);

        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<List<Models.Options.OptionTradeV3>>(response.Results);

        // If we have results, verify the first item structure
        if (response.Results.Count > 0)
        {
            var trade = response.Results[0];
            Assert.IsType<Models.Options.OptionTradeV3>(trade);

            if (trade.Price.HasValue)
                Assert.IsType<decimal>(trade.Price.Value);

            if (trade.Size.HasValue)
                Assert.IsType<int>(trade.Size.Value);

            if (trade.Exchange.HasValue)
                Assert.IsType<int>(trade.Exchange.Value);

            if (trade.SipTimestamp.HasValue)
                Assert.IsType<long>(trade.SipTimestamp.Value);

            if (trade.ParticipantTimestamp.HasValue)
                Assert.IsType<long>(trade.ParticipantTimestamp.Value);

            if (trade.SequenceNumber.HasValue)
                Assert.IsType<long>(trade.SequenceNumber.Value);

            if (trade.Conditions != null)
                Assert.IsType<List<int>>(trade.Conditions);

            if (trade.Id != null)
                Assert.IsType<string>(trade.Id);
        }
    }

    /// <summary>
    /// Tests that the client correctly handles invalid options tickers.
    /// Note: The trades endpoint may return an OK response with empty results instead of throwing for invalid tickers.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_ForInvalidTicker_ShouldHandleGracefully()
    {
        // Arrange
        var invalidTicker = "O:INVALID000000C00000000";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(invalidTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client can handle the response
        // The API may return OK with empty results or throw an exception depending on the ticker format
        Assert.NotNull(response);

        // If the response is OK, results should be either null or empty
        if (response.Status == "OK")
        {
            Assert.True(response.Results == null || response.Results.Count == 0,
                "Invalid ticker should return empty results if response is OK");
        }
    }

    /// <summary>
    /// Tests that the MarketTimestamp computed property converts SIP timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_MarketTimestamp_ShouldConvertToEasternTime()
    {
        // Arrange
        var optionsTicker = "O:TSLA210903C00700000";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(optionsTicker, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify MarketTimestamp conversion works
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0 && response.Results[0].SipTimestamp.HasValue)
        {
            Assert.NotNull(response.Results[0].MarketTimestamp);
            Assert.Equal("America/New_York", response.Results[0].MarketTimestamp.Value.Zone.Id);
        }
    }

    /// <summary>
    /// Tests that the MarketParticipantTimestamp computed property converts participant timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_MarketParticipantTimestamp_ShouldConvertToEasternTime()
    {
        // Arrange
        var optionsTicker = "O:TSLA210903C00700000";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(optionsTicker, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify MarketParticipantTimestamp conversion works
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0 && response.Results[0].ParticipantTimestamp.HasValue)
        {
            Assert.NotNull(response.Results[0].MarketParticipantTimestamp);
            Assert.Equal("America/New_York", response.Results[0].MarketParticipantTimestamp.Value.Zone.Id);
        }
    }

    /// <summary>
    /// Tests that pagination works correctly with next_url.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_WithPagination_ShouldReturnNextUrl()
    {
        // Arrange
        var optionsTicker = "O:TSLA210903C00700000";
        var optionsService = PolygonClient.Options;

        // Act - Request with small limit to ensure pagination
        var response = await optionsService.GetTradesAsync(optionsTicker, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify pagination structure exists (next_url may or may not be present depending on data)
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);

        // If there's a next_url, it should be a valid URL string
        if (!string.IsNullOrEmpty(response.NextUrl))
        {
            Assert.IsType<string>(response.NextUrl);
            Assert.StartsWith("https://", response.NextUrl);
        }
    }

    /// <summary>
    /// Tests that the client correctly deserializes trade prices and sizes.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_ShouldDeserializePriceAndSize()
    {
        // Arrange
        var optionsTicker = "O:TSLA210903C00700000";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(optionsTicker, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify price and size are properly deserialized
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            var trade = response.Results[0];
            // Price and size should typically be present
            Assert.True(trade.Price.HasValue, "Trade should have a price");
            Assert.True(trade.Size.HasValue, "Trade should have a size");

            // Price should be positive
            if (trade.Price.HasValue)
            {
                Assert.True(trade.Price.Value > 0, $"Trade price ({trade.Price.Value}) should be positive");
            }

            // Size should be positive
            if (trade.Size.HasValue)
            {
                Assert.True(trade.Size.Value > 0, $"Trade size ({trade.Size.Value}) should be positive");
            }
        }
    }
}
