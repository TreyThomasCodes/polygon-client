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
/// These tests require a valid Polygon API key to be configured in user secrets.
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

    /// <summary>
    /// Tests fetching SPY call option contract details and verifies the client can successfully
    /// make the request and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ForSPYCallOption_ShouldReturnValidResponse()
    {
        // Arrange
        // Using a SPY call option ticker (December 2025 expiration, $650 strike)
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
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<Models.Options.OptionsContract>>(response);

        // Verify response structure
        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<Models.Options.OptionsContract>(response.Results);

        // Verify string properties
        if (response.Results.Ticker != null)
            Assert.IsType<string>(response.Results.Ticker);

        if (response.Results.UnderlyingTicker != null)
            Assert.IsType<string>(response.Results.UnderlyingTicker);

        if (response.Results.ContractType != null)
            Assert.IsType<string>(response.Results.ContractType);

        if (response.Results.ExerciseStyle != null)
            Assert.IsType<string>(response.Results.ExerciseStyle);

        if (response.Results.ExpirationDate != null)
            Assert.IsType<string>(response.Results.ExpirationDate);

        if (response.Results.PrimaryExchange != null)
            Assert.IsType<string>(response.Results.PrimaryExchange);

        if (response.Results.Cfi != null)
            Assert.IsType<string>(response.Results.Cfi);

        // Verify numeric properties
        if (response.Results.StrikePrice.HasValue)
            Assert.IsType<decimal>(response.Results.StrikePrice.Value);

        if (response.Results.SharesPerContract.HasValue)
            Assert.IsType<int>(response.Results.SharesPerContract.Value);
    }

    /// <summary>
    /// Tests behavior when requesting contract details for an invalid options ticker.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "O:INVALID000000C00000000";
        var optionsService = _polygonClient.Options;

        // Act & Assert
        // The Polygon API returns a 404 for invalid option tickers
        // This is expected behavior and should throw a Refit.ApiException
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => optionsService.GetContractDetailsAsync(invalidTicker, TestContext.Current.CancellationToken));

        // Verify the exception details
        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }

    #region GetSnapshotAsync Integration Tests

    /// <summary>
    /// Tests fetching SPY call option snapshot and verifies the client can successfully
    /// make the request and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ForSPYCallOption_ShouldReturnValidResponse()
    {
        // Arrange
        // Using SPY as underlying asset and a December 2025 $650 call option
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

        // Assert
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<Models.Options.OptionSnapshot>>(response);

        // Verify response structure
        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<Models.Options.OptionSnapshot>(response.Results);

        // Verify top-level properties
        if (response.Results.BreakEvenPrice.HasValue)
            Assert.IsType<decimal>(response.Results.BreakEvenPrice.Value);

        if (response.Results.ImpliedVolatility.HasValue)
            Assert.IsType<decimal>(response.Results.ImpliedVolatility.Value);

        if (response.Results.OpenInterest.HasValue)
            Assert.IsType<int>(response.Results.OpenInterest.Value);

        // Verify Day data structure
        if (response.Results.Day != null)
        {
            Assert.IsType<Models.Options.OptionDayData>(response.Results.Day);
            if (response.Results.Day.Close.HasValue)
                Assert.IsType<decimal>(response.Results.Day.Close.Value);
            if (response.Results.Day.Volume.HasValue)
                Assert.IsType<long>(response.Results.Day.Volume.Value);
        }

        // Verify Details structure
        if (response.Results.Details != null)
        {
            Assert.IsType<Models.Options.OptionContractDetails>(response.Results.Details);
            if (response.Results.Details.ContractType != null)
                Assert.IsType<string>(response.Results.Details.ContractType);
            if (response.Results.Details.StrikePrice.HasValue)
                Assert.IsType<decimal>(response.Results.Details.StrikePrice.Value);
        }

        // Verify Greeks structure
        if (response.Results.Greeks != null)
        {
            Assert.IsType<Models.Options.OptionGreeks>(response.Results.Greeks);
            if (response.Results.Greeks.Delta.HasValue)
                Assert.IsType<decimal>(response.Results.Greeks.Delta.Value);
            if (response.Results.Greeks.Gamma.HasValue)
                Assert.IsType<decimal>(response.Results.Greeks.Gamma.Value);
        }

        // Verify LastQuote structure
        if (response.Results.LastQuote != null)
        {
            Assert.IsType<Models.Options.OptionLastQuote>(response.Results.LastQuote);
            if (response.Results.LastQuote.Ask.HasValue)
                Assert.IsType<decimal>(response.Results.LastQuote.Ask.Value);
            if (response.Results.LastQuote.Bid.HasValue)
                Assert.IsType<decimal>(response.Results.LastQuote.Bid.Value);
        }

        // Verify LastTrade structure
        if (response.Results.LastTrade != null)
        {
            Assert.IsType<Models.Options.OptionLastTrade>(response.Results.LastTrade);
            if (response.Results.LastTrade.Price.HasValue)
                Assert.IsType<decimal>(response.Results.LastTrade.Price.Value);
            if (response.Results.LastTrade.Size.HasValue)
                Assert.IsType<int>(response.Results.LastTrade.Size.Value);
        }

        // Verify UnderlyingAsset structure
        if (response.Results.UnderlyingAsset != null)
        {
            Assert.IsType<Models.Options.OptionUnderlyingAsset>(response.Results.UnderlyingAsset);
            if (response.Results.UnderlyingAsset.Price.HasValue)
                Assert.IsType<decimal>(response.Results.UnderlyingAsset.Price.Value);
            if (response.Results.UnderlyingAsset.Ticker != null)
                Assert.IsType<string>(response.Results.UnderlyingAsset.Ticker);
        }
    }

    /// <summary>
    /// Tests fetching put option snapshot to verify both call and put options work correctly.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ForSPYPutOption_ShouldReturnValidResponse()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionContract = "O:SPY251219P00650000"; // Put option
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        // Verify contract type is put
        if (response.Results.Details != null)
        {
            Assert.Equal("put", response.Results.Details.ContractType);
        }

        // Verify delta is negative for put options (if Greeks are available)
        if (response.Results.Greeks?.Delta.HasValue == true)
        {
            Assert.True(response.Results.Greeks.Delta.Value <= 0, "Put option delta should be negative or zero");
        }
    }

    /// <summary>
    /// Tests behavior when requesting snapshot for an invalid option contract.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_ForInvalidContract_ShouldThrowApiException()
    {
        // Arrange
        var underlyingAsset = "INVALID";
        var optionContract = "O:INVALID000000C00000000";
        var optionsService = _polygonClient.Options;

        // Act & Assert
        // The Polygon API returns a 404 for invalid option contracts
        // This is expected behavior and should throw a Refit.ApiException
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => optionsService.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken));

        // Verify the exception details
        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }

    /// <summary>
    /// Tests snapshot for multiple different underlying assets to verify client works across various tickers.
    /// </summary>
    [Theory]
    [InlineData("SPY", "O:SPY251219C00650000")]
    [InlineData("AAPL", "O:AAPL260116C00250000")]
    public async Task GetSnapshotAsync_ForDifferentUnderlyings_ShouldReturnValidResponse(
        string underlyingAsset, string optionContract)
    {
        // Arrange
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        // Verify underlying asset ticker matches request
        if (response.Results.UnderlyingAsset?.Ticker != null)
        {
            Assert.Equal(underlyingAsset, response.Results.UnderlyingAsset.Ticker);
        }
    }

    #endregion

    #region GetChainSnapshotAsync Integration Tests

    /// <summary>
    /// Tests fetching option chain snapshot for MSTR and verifies the client can successfully
    /// make the request and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_ForMSTR_ShouldReturnValidResponse()
    {
        // Arrange
        var underlyingAsset = "MSTR";
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
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(underlyingAsset, limit: 5, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<List<Models.Options.OptionSnapshot>>>(response);

        // Verify response structure
        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<List<Models.Options.OptionSnapshot>>(response.Results);

        // If we have results, verify the structure of the first item
        if (response.Results.Count > 0)
        {
            var snapshot = response.Results[0];
            Assert.IsType<Models.Options.OptionSnapshot>(snapshot);

            // Verify top-level properties
            if (snapshot.BreakEvenPrice.HasValue)
                Assert.IsType<decimal>(snapshot.BreakEvenPrice.Value);

            if (snapshot.ImpliedVolatility.HasValue)
                Assert.IsType<decimal>(snapshot.ImpliedVolatility.Value);

            if (snapshot.OpenInterest.HasValue)
                Assert.IsType<int>(snapshot.OpenInterest.Value);

            // Verify nested structures exist
            if (snapshot.Details != null)
                Assert.IsType<Models.Options.OptionContractDetails>(snapshot.Details);

            if (snapshot.Greeks != null)
                Assert.IsType<Models.Options.OptionGreeks>(snapshot.Greeks);

            if (snapshot.UnderlyingAsset != null)
                Assert.IsType<Models.Options.OptionUnderlyingAsset>(snapshot.UnderlyingAsset);
        }
    }

    /// <summary>
    /// Tests fetching option chain snapshot with strike price filter.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithStrikePrice_ShouldReturnFilteredResults()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var strikePrice = 650m;
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(
            underlyingAsset,
            strikePrice: strikePrice,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        // Verify all results have the specified strike price
        foreach (var snapshot in response.Results)
        {
            if (snapshot.Details?.StrikePrice.HasValue == true)
            {
                Assert.Equal(strikePrice, snapshot.Details.StrikePrice.Value);
            }
        }
    }

    /// <summary>
    /// Tests fetching option chain snapshot with contract type filter.
    /// </summary>
    [Theory]
    [InlineData("call")]
    [InlineData("put")]
    public async Task GetChainSnapshotAsync_WithContractType_ShouldReturnFilteredResults(string contractType)
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(
            underlyingAsset,
            contractType: contractType,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        // Verify all results have the specified contract type
        foreach (var snapshot in response.Results)
        {
            if (snapshot.Details?.ContractType != null)
            {
                Assert.Equal(contractType, snapshot.Details.ContractType);
            }
        }
    }

    /// <summary>
    /// Tests fetching option chain snapshot with expiration date filter.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithExpirationDateFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var expirationDateGte = "2025-10-01";
        var expirationDateLte = "2025-12-31";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(
            underlyingAsset,
            expirationDateGte: expirationDateGte,
            expirationDateLte: expirationDateLte,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        // Verify all results have expiration dates within the specified range
        foreach (var snapshot in response.Results)
        {
            if (snapshot.Details?.ExpirationDate != null)
            {
                // Parse the expiration date and verify it's within range
                var expirationDate = DateTime.Parse(snapshot.Details.ExpirationDate);
                var minDate = DateTime.Parse(expirationDateGte);
                var maxDate = DateTime.Parse(expirationDateLte);

                Assert.True(expirationDate >= minDate, $"Expiration date {expirationDate:yyyy-MM-dd} should be >= {expirationDateGte}");
                Assert.True(expirationDate <= maxDate, $"Expiration date {expirationDate:yyyy-MM-dd} should be <= {expirationDateLte}");
            }
        }
    }

    /// <summary>
    /// Tests fetching option chain snapshot with pagination support.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithPagination_ShouldReturnNextUrl()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var limit = 5; // Small limit to ensure pagination
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(
            underlyingAsset,
            limit: limit,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);

        // If there are more results than the limit, we should get a next_url
        // Note: This may or may not be present depending on the total number of contracts available
        if (response.Results?.Count >= limit)
        {
            // We expect a NextUrl if we got the full limit
            // (though it's possible there are exactly 'limit' results and no more)
            Assert.NotNull(response.NextUrl);
        }
    }

    /// <summary>
    /// Tests behavior when requesting option chain snapshot for an invalid underlying asset.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_ForInvalidUnderlying_ShouldReturnEmptyResults()
    {
        // Arrange
        var invalidUnderlying = "INVALIDTICKER999";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(invalidUnderlying, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        // For invalid underlying assets, the API returns OK status with empty results
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.Empty(response.Results);
    }

    /// <summary>
    /// Tests fetching option chain snapshot for multiple different underlying assets.
    /// </summary>
    [Theory]
    [InlineData("SPY")]
    [InlineData("AAPL")]
    [InlineData("TSLA")]
    public async Task GetChainSnapshotAsync_ForDifferentUnderlyings_ShouldReturnValidResponse(string underlyingAsset)
    {
        // Arrange
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(
            underlyingAsset,
            limit: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        // Verify underlying asset ticker matches request in all results
        foreach (var snapshot in response.Results)
        {
            if (snapshot.UnderlyingAsset?.Ticker != null)
            {
                Assert.Equal(underlyingAsset, snapshot.UnderlyingAsset.Ticker);
            }
        }
    }

    /// <summary>
    /// Tests fetching option chain snapshot with sorting and ordering.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithSortingAndOrdering_ShouldReturnSortedResults()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotAsync(
            underlyingAsset,
            limit: 10,
            order: "asc",
            sort: "strike_price",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        // Verify results are sorted by strike price in ascending order
        if (response.Results.Count > 1)
        {
            for (int i = 0; i < response.Results.Count - 1; i++)
            {
                var currentStrike = response.Results[i].Details?.StrikePrice;
                var nextStrike = response.Results[i + 1].Details?.StrikePrice;

                if (currentStrike.HasValue && nextStrike.HasValue)
                {
                    Assert.True(currentStrike.Value <= nextStrike.Value,
                        $"Strike prices should be in ascending order: {currentStrike} <= {nextStrike}");
                }
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
