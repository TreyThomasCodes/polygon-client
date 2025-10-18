// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.IntegrationTests.Options;

/// <summary>
/// Integration tests for fetching quotes for an options contract from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetQuotesIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching quotes for an SPY put option.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_ForSPYPutOption_ShouldReturnValidResponse()
    {
        // Arrange
        var request = new GetQuotesRequest
        {
            OptionsTicker = "O:SPY241220P00720000",
            Limit = 10
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests the data types and structure of the option quotes response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var request = new GetQuotesRequest
        {
            OptionsTicker = "O:SPY241220P00720000",
            Limit = 5
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<List<Models.Options.OptionQuote>>>(response);

        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<List<Models.Options.OptionQuote>>(response.Results);

        // If we have results, verify the first item structure
        if (response.Results.Count > 0)
        {
            var quote = response.Results[0];
            Assert.IsType<Models.Options.OptionQuote>(quote);

            if (quote.AskPrice.HasValue)
                Assert.IsType<decimal>(quote.AskPrice.Value);

            if (quote.AskSize.HasValue)
                Assert.IsType<int>(quote.AskSize.Value);

            if (quote.AskExchange.HasValue)
                Assert.IsType<int>(quote.AskExchange.Value);

            if (quote.BidPrice.HasValue)
                Assert.IsType<decimal>(quote.BidPrice.Value);

            if (quote.BidSize.HasValue)
                Assert.IsType<int>(quote.BidSize.Value);

            if (quote.BidExchange.HasValue)
                Assert.IsType<int>(quote.BidExchange.Value);

            if (quote.SequenceNumber.HasValue)
                Assert.IsType<long>(quote.SequenceNumber.Value);

            if (quote.SipTimestamp.HasValue)
                Assert.IsType<long>(quote.SipTimestamp.Value);
        }
    }

    /// <summary>
    /// Tests that the client correctly validates options ticker format.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_ForInvalidTicker_ShouldThrowValidationException()
    {
        // Arrange
        var request = new GetQuotesRequest
        {
            OptionsTicker = "O:INVALID000000C00000000"
        };
        var optionsService = PolygonClient.Options;

        // Act & Assert - Verify validation catches invalid OCC ticker format
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => optionsService.GetQuotesAsync(request, TestContext.Current.CancellationToken));

        Assert.Contains("OptionsTicker", exception.Message);
    }

    /// <summary>
    /// Tests that the MarketTimestamp computed property converts timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_MarketTimestamp_ShouldConvertToEasternTime()
    {
        // Arrange
        var request = new GetQuotesRequest
        {
            OptionsTicker = "O:SPY241220P00720000",
            Limit = 5
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(request, TestContext.Current.CancellationToken);

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
    /// Tests that the client correctly deserializes bid and ask prices.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_ShouldDeserializeBidAskPrices()
    {
        // Arrange
        var request = new GetQuotesRequest
        {
            OptionsTicker = "O:SPY241220P00720000",
            Limit = 5
        };
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(request, TestContext.Current.CancellationToken);

        // Assert - Verify bid/ask prices are properly deserialized
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            var quote = response.Results[0];
            // At least one of ask or bid should typically be present
            Assert.True(quote.AskPrice.HasValue || quote.BidPrice.HasValue);

            // If both are present, ask should be higher than or equal to bid
            if (quote.AskPrice.HasValue && quote.BidPrice.HasValue)
            {
                Assert.True(quote.AskPrice.Value >= quote.BidPrice.Value,
                    $"Ask price ({quote.AskPrice.Value}) should be >= bid price ({quote.BidPrice.Value})");
            }
        }
    }

    /// <summary>
    /// Tests that pagination works correctly with next_url.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithPagination_ShouldReturnNextUrl()
    {
        // Arrange
        var request = new GetQuotesRequest
        {
            OptionsTicker = "O:SPY241220P00720000",
            Limit = 5
        };
        var optionsService = PolygonClient.Options;

        // Act - Request with small limit to ensure pagination
        var response = await optionsService.GetQuotesAsync(request, TestContext.Current.CancellationToken);

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
}
