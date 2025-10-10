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
