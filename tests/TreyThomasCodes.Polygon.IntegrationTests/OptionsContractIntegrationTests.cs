// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching options contract data from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class OptionsContractIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the OptionsContractIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public OptionsContractIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<OptionsContractIntegrationTests>();
        var apiKey = builder.Configuration["Polygon:ApiKey"];

        // Skip all tests in this class if no API key is configured
        Assert.SkipUnless(!string.IsNullOrEmpty(apiKey), "Polygon API key not configured in user secrets. Use: dotnet user-secrets set \"Polygon:ApiKey\" \"your-api-key-here\"");

        _apiKey = apiKey!; // Safe to use ! since we skip if null or empty

        builder.Services.AddPolygonClient(options =>
        {
            options.ApiKey = _apiKey;
        });

        _host = builder.Build();
        _polygonClient = _host.Services.GetRequiredService<IPolygonClient>();
    }

    #region GetContractDetailsAsync Integration Tests

    /// <summary>
    /// Tests fetching SPY call option contract details.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ForSPYCallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests the data types and structure of the options contract response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<Models.Options.OptionsContract>>(response);

        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<Models.Options.OptionsContract>(response.Results);
    }

    /// <summary>
    /// Tests that the client correctly handles errors for invalid options tickers.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "O:INVALID000000C00000000";
        var optionsService = _polygonClient.Options;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => optionsService.GetContractDetailsAsync(invalidTicker, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }

    #endregion

    #region GetSnapshotAsync Integration Tests

    /// <summary>
    /// Tests fetching SPY call option snapshot.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ForSPYCallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionContract = "O:SPY251219C00650000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests the data types and structure of the options snapshot response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionContract = "O:SPY251219C00650000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<Models.Options.OptionSnapshot>>(response);

        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<Models.Options.OptionSnapshot>(response.Results);

        // Verify nested structures exist with correct types if present
        if (response.Results.Day != null)
            Assert.IsType<Models.Options.OptionDayData>(response.Results.Day);

        if (response.Results.Details != null)
            Assert.IsType<Models.Options.OptionContractDetails>(response.Results.Details);

        if (response.Results.Greeks != null)
            Assert.IsType<Models.Options.OptionGreeks>(response.Results.Greeks);

        if (response.Results.LastQuote != null)
            Assert.IsType<Models.Options.OptionLastQuote>(response.Results.LastQuote);

        if (response.Results.LastTrade != null)
            Assert.IsType<Models.Options.OptionLastTrade>(response.Results.LastTrade);

        if (response.Results.UnderlyingAsset != null)
            Assert.IsType<Models.Options.OptionUnderlyingAsset>(response.Results.UnderlyingAsset);
    }

    /// <summary>
    /// Tests that the client correctly handles errors for invalid option contracts.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ForInvalidContract_ShouldThrowApiException()
    {
        // Arrange
        var underlyingAsset = "INVALID";
        var optionContract = "O:INVALID000000C00000000";
        var optionsService = _polygonClient.Options;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => optionsService.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }

    #endregion

    #region GetChainSnapshotAsync Integration Tests

    /// <summary>
    /// Tests fetching option chain snapshot for SPY.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_ForSPY_ShouldReturnValidResponse()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(underlyingAsset, limit: 10, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests the data types and structure of the option chain snapshot response.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(underlyingAsset, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<List<Models.Options.OptionSnapshot>>>(response);

        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<List<Models.Options.OptionSnapshot>>(response.Results);

        // If we have results, verify the first item structure
        if (response.Results.Count > 0)
        {
            var snapshot = response.Results[0];
            Assert.IsType<Models.Options.OptionSnapshot>(snapshot);

            if (snapshot.Details != null)
                Assert.IsType<Models.Options.OptionContractDetails>(snapshot.Details);

            if (snapshot.Greeks != null)
                Assert.IsType<Models.Options.OptionGreeks>(snapshot.Greeks);

            if (snapshot.UnderlyingAsset != null)
                Assert.IsType<Models.Options.OptionUnderlyingAsset>(snapshot.UnderlyingAsset);
        }
    }

    #endregion

    #region GetLastTradeAsync Integration Tests

    /// <summary>
    /// Tests fetching last trade for a TSLA call option.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForTSLACallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:TSLA260320C00700000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

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
        var optionsTicker = "O:TSLA260320C00700000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

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
    /// Tests that the client correctly handles errors for invalid options tickers.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "O:INVALID000000C00000000";
        var optionsService = _polygonClient.Options;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => optionsService.GetLastTradeAsync(invalidTicker, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }

    /// <summary>
    /// Tests that the client correctly deserializes the ticker property in the response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ShouldDeserializeTickerProperty()
    {
        // Arrange
        var optionsTicker = "O:TSLA260320C00700000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert - Verify ticker is properly deserialized
        Assert.NotNull(response);
        Assert.NotNull(response.Results);
        Assert.NotNull(response.Results.Ticker);
        Assert.Equal(optionsTicker, response.Results.Ticker);
    }

    /// <summary>
    /// Tests that the MarketTimestamp computed property converts timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_MarketTimestamp_ShouldConvertToEasternTime()
    {
        // Arrange
        var optionsTicker = "O:TSLA260320C00700000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert - Verify MarketTimestamp conversion works
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        if (response.Results.Timestamp.HasValue)
        {
            Assert.NotNull(response.Results.MarketTimestamp);
            Assert.Equal("America/New_York", response.Results.MarketTimestamp.Value.Zone.Id);
        }
    }

    #endregion

    #region GetQuotesAsync Integration Tests

    /// <summary>
    /// Tests fetching quotes for an SPY put option.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_ForSPYPutOption_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(optionsTicker, limit: 10, cancellationToken: TestContext.Current.CancellationToken);

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
        var optionsTicker = "O:SPY241220P00720000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(optionsTicker, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

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
    /// Tests that the client correctly handles invalid options tickers.
    /// Note: The quotes endpoint may return an OK response with empty results instead of throwing for invalid tickers.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_ForInvalidTicker_ShouldHandleGracefully()
    {
        // Arrange
        var invalidTicker = "O:INVALID000000C00000000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(invalidTicker, cancellationToken: TestContext.Current.CancellationToken);

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
    /// Tests that the MarketTimestamp computed property converts timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_MarketTimestamp_ShouldConvertToEasternTime()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(optionsTicker, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

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
        var optionsTicker = "O:SPY241220P00720000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(optionsTicker, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

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
        var optionsTicker = "O:SPY241220P00720000";
        var optionsService = _polygonClient.Options;

        // Act - Request with small limit to ensure pagination
        var response = await optionsService.GetQuotesAsync(optionsTicker, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

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

    #endregion

    #region GetTradesAsync Integration Tests

    /// <summary>
    /// Tests fetching trades for a TSLA call option.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_ForTSLACallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:TSLA210903C00700000";
        var optionsService = _polygonClient.Options;

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
        var optionsService = _polygonClient.Options;

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
        var optionsService = _polygonClient.Options;

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
        var optionsService = _polygonClient.Options;

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
        var optionsService = _polygonClient.Options;

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
        var optionsService = _polygonClient.Options;

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
        var optionsService = _polygonClient.Options;

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

    #endregion

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _host.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
