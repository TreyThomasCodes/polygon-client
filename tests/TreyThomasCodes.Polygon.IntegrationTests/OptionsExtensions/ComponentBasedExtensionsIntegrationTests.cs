// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.IntegrationTests.OptionsExtensions;

/// <summary>
/// Integration tests for component-based extension methods in OptionsServiceExtensions.
/// These tests verify that extension methods accepting individual components (underlying, expiration, type, strike)
/// properly delegate to the underlying API and construct valid options tickers.
/// </summary>
public class ComponentBasedExtensionsIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that GetContractByComponentsAsync constructs the ticker correctly and returns valid data.
    /// Verifies the extension method properly delegates to GetContractDetailsAsync.
    /// </summary>
    [Fact]
    public async Task GetContractByComponentsAsync_WithValidComponents_ShouldReturnValidResponse()
    {
        // Arrange
        var underlying = "TSLA";
        var expirationDate = new DateOnly(2026, 3, 20);
        var type = OptionType.Call;
        var strike = 700m;
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetContractByComponentsAsync(
            underlying,
            expirationDate,
            type,
            strike,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);

        // Verify the contract details match the requested components
        Assert.Equal(underlying, response.Results.UnderlyingTicker);
        Assert.Equal("call", response.Results.ContractType);
        Assert.Equal(strike, response.Results.StrikePrice);
    }

    /// <summary>
    /// Tests that GetContractByComponentsAsync constructs the correct OCC ticker format.
    /// </summary>
    [Fact]
    public async Task GetContractByComponentsAsync_ShouldConstructCorrectOccTicker()
    {
        // Arrange
        var underlying = "SPY";
        var expirationDate = new DateOnly(2025, 12, 19);
        var type = OptionType.Call;
        var strike = 650m;
        var expectedTicker = "O:SPY251219C00650000";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetContractByComponentsAsync(
            underlying,
            expirationDate,
            type,
            strike,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);
        Assert.Equal(expectedTicker, response.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetSnapshotByComponentsAsync constructs the ticker correctly and returns valid snapshot data.
    /// Note: Snapshot endpoint requires the option contract to be currently available in the market.
    /// </summary>
    [Fact]
    public async Task GetSnapshotByComponentsAsync_WithValidComponents_ShouldReturnValidResponse()
    {
        // Arrange - use SPY which is a heavily traded ticker with reliable snapshot data
        var underlying = "SPY";
        var expirationDate = new DateOnly(2025, 12, 19);
        var type = OptionType.Call;
        var strike = 650m;
        var optionsService = PolygonClient.Options;

        try
        {
            // Act
            var response = await optionsService.GetSnapshotByComponentsAsync(
                underlying,
                expirationDate,
                type,
                strike,
                TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("OK", response.Status);
            Assert.NotNull(response.RequestId);
            Assert.NotNull(response.Results);

            // Verify snapshot contains expected data structures
            Assert.NotNull(response.Results.Details);
            Assert.Equal(strike, response.Results.Details.StrikePrice);
            Assert.Contains(underlying, response.Results.Details.Ticker);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Contract may not be available for snapshot - skip test
            Assert.SkipWhen(true, "Option contract not available for snapshot in current market");
        }
    }

    /// <summary>
    /// Tests that GetSnapshotByComponentsAsync works with PUT options.
    /// Note: Snapshot endpoint requires the option contract to be currently available in the market.
    /// </summary>
    [Fact]
    public async Task GetSnapshotByComponentsAsync_WithPutOption_ShouldReturnValidResponse()
    {
        // Arrange - use SPY which is a heavily traded ticker with reliable snapshot data
        var underlying = "SPY";
        var expirationDate = new DateOnly(2025, 12, 19);
        var type = OptionType.Put;
        var strike = 550m;
        var optionsService = PolygonClient.Options;

        try
        {
            // Act
            var response = await optionsService.GetSnapshotByComponentsAsync(
                underlying,
                expirationDate,
                type,
                strike,
                TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("OK", response.Status);
            Assert.NotNull(response.RequestId);
            Assert.NotNull(response.Results);

            // Verify snapshot contains expected data structures
            Assert.NotNull(response.Results.Details);
            Assert.Equal(strike, response.Results.Details.StrikePrice);
            Assert.Equal("put", response.Results.Details.ContractType);
            Assert.Contains(underlying, response.Results.Details.Ticker);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Contract may not be available for snapshot - skip test
            Assert.SkipWhen(true, "Option contract not available for snapshot in current market");
        }
    }

    /// <summary>
    /// Tests that GetLastTradeByComponentsAsync constructs the ticker correctly and returns valid trade data.
    /// </summary>
    [Fact]
    public async Task GetLastTradeByComponentsAsync_WithValidComponents_ShouldReturnValidResponse()
    {
        // Arrange
        var underlying = "TSLA";
        var expirationDate = new DateOnly(2026, 3, 20);
        var type = OptionType.Call;
        var strike = 700m;
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeByComponentsAsync(
            underlying,
            expirationDate,
            type,
            strike,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotNull(response.Results);

        // Verify trade data
        Assert.NotNull(response.Results.Ticker);
        Assert.Contains("TSLA", response.Results.Ticker);
        Assert.Contains("C", response.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetBarsByComponentsAsync constructs the ticker correctly and returns valid bar data.
    /// </summary>
    [Fact]
    public async Task GetBarsByComponentsAsync_WithDailyInterval_ShouldReturnValidResponse()
    {
        // Arrange
        var underlying = "SPY";
        var expirationDate = new DateOnly(2025, 12, 19);
        var type = OptionType.Call;
        var strike = 650m;
        var multiplier = 1;
        var timespan = AggregateInterval.Day;
        var from = new DateOnly(2023, 1, 9);
        var to = new DateOnly(2023, 2, 10);
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsByComponentsAsync(
            underlying,
            expirationDate,
            type,
            strike,
            multiplier,
            timespan,
            from,
            to,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotNull(response.Results);

        // Verify bars contain OHLC data
        if (response.Results.Count > 0)
        {
            var bar = response.Results[0];
            Assert.True(bar.Open.HasValue || bar.High.HasValue || bar.Low.HasValue || bar.Close.HasValue);
        }
    }

    /// <summary>
    /// Tests that GetBarsByComponentsAsync works with minute intervals.
    /// </summary>
    [Fact]
    public async Task GetBarsByComponentsAsync_WithMinuteInterval_ShouldReturnValidResponse()
    {
        // Arrange
        var underlying = "SPY";
        var expirationDate = new DateOnly(2025, 12, 19);
        var type = OptionType.Call;
        var strike = 650m;
        var multiplier = 5;
        var timespan = AggregateInterval.Minute;
        var from = new DateOnly(2023, 1, 9);
        var to = new DateOnly(2023, 1, 10);
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetBarsByComponentsAsync(
            underlying,
            expirationDate,
            type,
            strike,
            multiplier,
            timespan,
            from,
            to,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
    }

    /// <summary>
    /// Tests that GetChainSnapshotByComponentsAsync returns a list of option snapshots for the underlying asset.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotByComponentsAsync_WithValidUnderlying_ShouldReturnValidResponse()
    {
        // Arrange
        var underlying = "SPY";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotByComponentsAsync(
            underlying,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);
        Assert.NotNull(response.Results);
        Assert.NotEmpty(response.Results);

        // Verify all results are for the correct underlying
        foreach (var snapshot in response.Results)
        {
            Assert.NotNull(snapshot.Details);
            Assert.Contains(underlying, snapshot.Details.Ticker);
        }
    }

    /// <summary>
    /// Tests that GetChainSnapshotByComponentsAsync correctly filters by option type.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotByComponentsAsync_WithOptionTypeFilter_ShouldReturnOnlyMatchingType()
    {
        // Arrange
        var underlying = "SPY";
        var type = OptionType.Call;
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotByComponentsAsync(
            underlying,
            type: type,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.NotEmpty(response.Results);

        // Verify all results are call options
        foreach (var snapshot in response.Results)
        {
            Assert.NotNull(snapshot.Details);
            Assert.Equal("call", snapshot.Details.ContractType);
        }
    }

    /// <summary>
    /// Tests that GetChainSnapshotByComponentsAsync correctly filters by expiration date range.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotByComponentsAsync_WithExpirationDateRange_ShouldReturnMatchingContracts()
    {
        // Arrange
        var underlying = "SPY";
        var expirationDateGte = new DateOnly(2025, 12, 1);
        var expirationDateLte = new DateOnly(2025, 12, 31);
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotByComponentsAsync(
            underlying,
            expirationDateGte: expirationDateGte,
            expirationDateLte: expirationDateLte,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            // Verify expiration dates are within the specified range
            foreach (var snapshot in response.Results)
            {
                Assert.NotNull(snapshot.Details);
                Assert.NotNull(snapshot.Details.ExpirationDate);

                var expirationDate = DateOnly.Parse(snapshot.Details.ExpirationDate);
                Assert.True(expirationDate >= expirationDateGte, $"Expiration date {expirationDate} should be >= {expirationDateGte}");
                Assert.True(expirationDate <= expirationDateLte, $"Expiration date {expirationDate} should be <= {expirationDateLte}");
            }
        }
    }

    /// <summary>
    /// Tests that GetChainSnapshotByComponentsAsync correctly filters by strike price.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotByComponentsAsync_WithStrikePriceFilter_ShouldReturnOnlyMatchingStrike()
    {
        // Arrange
        var underlying = "SPY";
        var strikePrice = 650m;
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotByComponentsAsync(
            underlying,
            strikePrice: strikePrice,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            // Verify all results have the specified strike price
            foreach (var snapshot in response.Results)
            {
                Assert.NotNull(snapshot.Details);
                Assert.Equal(strikePrice, snapshot.Details.StrikePrice);
            }
        }
    }

    /// <summary>
    /// Tests that GetChainSnapshotByComponentsAsync works with multiple filters combined.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotByComponentsAsync_WithMultipleFilters_ShouldReturnMatchingContracts()
    {
        // Arrange
        var underlying = "SPY";
        var type = OptionType.Put;
        var expirationDateGte = new DateOnly(2025, 12, 1);
        var expirationDateLte = new DateOnly(2025, 12, 31);
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetChainSnapshotByComponentsAsync(
            underlying,
            type: type,
            expirationDateGte: expirationDateGte,
            expirationDateLte: expirationDateLte,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        if (response.Results.Count > 0)
        {
            // Verify all results match all filters
            foreach (var snapshot in response.Results)
            {
                Assert.NotNull(snapshot.Details);
                Assert.Equal("put", snapshot.Details.ContractType);
                Assert.Contains(underlying, snapshot.Details.Ticker);
                Assert.NotNull(snapshot.Details.ExpirationDate);

                var expirationDate = DateOnly.Parse(snapshot.Details.ExpirationDate);
                Assert.True(expirationDate >= expirationDateGte);
                Assert.True(expirationDate <= expirationDateLte);
            }
        }
    }
}
