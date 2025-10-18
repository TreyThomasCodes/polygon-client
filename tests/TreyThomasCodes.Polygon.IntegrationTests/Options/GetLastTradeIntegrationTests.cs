// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.RestClient.Exceptions;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.IntegrationTests.Options;

/// <summary>
/// Integration tests for fetching the last trade for an options contract from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetLastTradeIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching last trade for a TSLA call option.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForTSLACallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            OptionsTicker = "O:TSLA260320C00700000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests the data types and structure of the option last trade response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            OptionsTicker = "O:TSLA260320C00700000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<Models.Options.OptionTrade>>(response);

        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<Models.Options.OptionTrade>(response.Results);

        // Verify key properties exist with correct types
        if (response.Results.Ticker != null)
            Assert.IsType<string>(response.Results.Ticker);

        if (response.Results.Price.HasValue)
            Assert.IsType<decimal>(response.Results.Price.Value);

        if (response.Results.Size.HasValue)
            Assert.IsType<int>(response.Results.Size.Value);

        if (response.Results.Timestamp.HasValue)
            Assert.IsType<long>(response.Results.Timestamp.Value);

        if (response.Results.Exchange.HasValue)
            Assert.IsType<int>(response.Results.Exchange.Value);

        if (response.Results.Conditions != null)
            Assert.IsType<List<int>>(response.Results.Conditions);

        if (response.Results.Sequence.HasValue)
            Assert.IsType<long>(response.Results.Sequence.Value);
    }

    /// <summary>
    /// Tests that the client correctly validates options ticker format.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForInvalidTicker_ShouldThrowValidationException()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            OptionsTicker = "O:INVALID000000C00000000"
        };
        var optionsService = PolygonClient.Options;

        // Act & Assert - Verify validation catches invalid OCC ticker format
        var exception = await Assert.ThrowsAsync<PolygonValidationException>(
            () => optionsService.GetLastTradeAsync(request, TestContext.Current.CancellationToken));

        Assert.Contains("OptionsTicker", exception.Message);
    }

    /// <summary>
    /// Tests that the client correctly deserializes the ticker property in the response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ShouldDeserializeTickerProperty()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            OptionsTicker = "O:TSLA260320C00700000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify ticker is properly deserialized
        Assert.NotNull(response);
        Assert.NotNull(response.Results);
        Assert.NotNull(response.Results.Ticker);
        Assert.Equal(request.OptionsTicker, response.Results.Ticker);
    }

    /// <summary>
    /// Tests that the MarketTimestamp computed property converts timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_MarketTimestamp_ShouldConvertToEasternTime()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            OptionsTicker = "O:TSLA260320C00700000"
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify MarketTimestamp conversion works
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Timestamp.HasValue)
        {
            Assert.NotNull(response.Results.MarketTimestamp);
            Assert.Equal("America/New_York", response.Results.MarketTimestamp.Value.Zone.Id);
        }
    }
}
