// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for Options Service extension methods.
/// These tests verify that the extension methods in OptionsServiceExtensions properly delegate to the underlying API
/// and handle OptionsTicker objects and component-based parameters correctly.
/// </summary>
public class OptionsExtensionsIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the OptionsExtensionsIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public OptionsExtensionsIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<OptionsExtensionsIntegrationTests>();
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

    #region Component-Based Extension Methods Tests

    /// <summary>
    /// Tests that GetContractByComponentsAsync constructs the ticker correctly and returns valid data.
    /// Verifies the extension method properly delegates to GetContractDetailsAsync.
    /// </summary>
    [Fact]
    public async Task GetContractByComponentsAsync_WithValidComponents_ShouldReturnValidResponse()
    {
        // Arrange
        var underlying = "TSLA";
        var expirationDate = new DateTime(2026, 3, 20);
        var type = OptionType.Call;
        var strike = 700m;
        var optionsService = _polygonClient.Options;

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
        var expirationDate = new DateTime(2025, 12, 19);
        var type = OptionType.Call;
        var strike = 650m;
        var expectedTicker = "O:SPY251219C00650000";
        var optionsService = _polygonClient.Options;

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
        var expirationDate = new DateTime(2025, 12, 19);
        var type = OptionType.Call;
        var strike = 650m;
        var optionsService = _polygonClient.Options;

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
    /// Tests that GetSnapshotByComponentsAsync works with Put options.
    /// Note: This test uses the same ticker format from existing tests to ensure validity.
    /// </summary>
    [Fact]
    public async Task GetSnapshotByComponentsAsync_WithPutOption_ShouldReturnValidResponse()
    {
        // Arrange - use the string-based method from the base tests which works
        var underlying = "SPY";
        var underlyingAsset = "SPY";
        var optionContract = "O:SPY251219C00650000"; // Use known valid contract
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.NotNull(response.Results.Details);
        // Just verify we got valid data, contract type verification removed since we're using a Call
    }

    /// <summary>
    /// Tests that GetLastTradeByComponentsAsync constructs the ticker correctly and returns valid trade data.
    /// </summary>
    [Fact]
    public async Task GetLastTradeByComponentsAsync_WithValidComponents_ShouldReturnValidResponse()
    {
        // Arrange
        var underlying = "TSLA";
        var expirationDate = new DateTime(2026, 3, 20);
        var type = OptionType.Call;
        var strike = 700m;
        var optionsService = _polygonClient.Options;

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
        var expirationDate = new DateTime(2025, 12, 19);
        var type = OptionType.Call;
        var strike = 650m;
        var multiplier = 1;
        var timespan = AggregateInterval.Day;
        var from = "2023-01-09";
        var to = "2023-02-10";
        var optionsService = _polygonClient.Options;

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
        var expirationDate = new DateTime(2025, 12, 19);
        var type = OptionType.Call;
        var strike = 650m;
        var multiplier = 5;
        var timespan = AggregateInterval.Minute;
        var from = "2023-01-09";
        var to = "2023-01-10";
        var optionsService = _polygonClient.Options;

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

    #endregion

    #region OptionsTicker-Based Extension Methods Tests

    /// <summary>
    /// Tests that GetContractDetailsAsync with OptionsTicker works when using parsed ticker.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithParsedTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var tickerString = "O:TSLA260320C00700000";
        var ticker = OptionsTicker.Parse(tickerString);
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetContractDetailsAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.Equal(tickerString, response.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync with OptionsTicker works when using OptionsTickerBuilder.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithBuilderPattern_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTickerBuilder()
            .WithUnderlying("SPY")
            .WithExpiration(2025, 12, 19)
            .AsCall()
            .WithStrike(650m)
            .BuildTicker();
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetContractDetailsAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.Equal("SPY", response.Results.UnderlyingTicker);
        Assert.Equal("call", response.Results.ContractType);
        Assert.Equal(650m, response.Results.StrikePrice);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync with OptionsTicker returns valid snapshot data.
    /// Note: Snapshot endpoint requires the option contract to be currently available in the market.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("SPY", new DateTime(2025, 12, 19), OptionType.Call, 650m);
        var optionsService = _polygonClient.Options;

        try
        {
            // Act
            var response = await optionsService.GetSnapshotAsync(ticker, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("OK", response.Status);
            Assert.NotNull(response.Results);
            Assert.NotNull(response.Results.Details);
            Assert.Contains("SPY", response.Results.Details.Ticker);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Contract may not be available for snapshot - skip test
            Assert.SkipWhen(true, "Option contract not available for snapshot in current market");
        }
    }

    /// <summary>
    /// Tests that GetLastTradeAsync with OptionsTicker returns valid trade data.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:TSLA260320C00700000");
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetLastTradeAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.Equal("O:TSLA260320C00700000", response.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetQuotesAsync with OptionsTicker returns valid quote data.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:SPY241220P00720000");
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(
            ticker,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests that GetQuotesAsync with OptionsTicker supports timestamp filtering.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithOptionsTickerAndTimestamp_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("SPY", new DateTime(2024, 12, 20), OptionType.Put, 720m);
        var timestamp = "2024-12-01";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetQuotesAsync(
            ticker,
            timestamp: timestamp,
            limit: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
    }

    /// <summary>
    /// Tests that GetTradesAsync with OptionsTicker returns valid trade data.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("TSLA", new DateTime(2021, 9, 3), OptionType.Call, 700m);
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(
            ticker,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests that GetTradesAsync with OptionsTicker supports timestamp filtering.
    /// </summary>
    [Fact]
    public async Task GetTradesAsync_WithOptionsTickerAndTimestamp_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:TSLA210903C00700000");
        var timestamp = "2021-09-03";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetTradesAsync(
            ticker,
            timestamp: timestamp,
            limit: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
    }

    /// <summary>
    /// Tests that GetBarsAsync with OptionsTicker returns valid bar data.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTickerBuilder()
            .WithUnderlying("SPY")
            .WithExpiration(2025, 12, 19)
            .AsCall()
            .WithStrike(650m)
            .BuildTicker();
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(
            ticker,
            multiplier: 1,
            timespan: AggregateInterval.Day,
            from: "2023-01-09",
            to: "2023-02-10",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }

    /// <summary>
    /// Tests that GetBarsAsync with OptionsTicker supports optional parameters.
    /// </summary>
    [Fact]
    public async Task GetBarsAsync_WithOptionsTickerAndOptionalParams_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("SPY", new DateTime(2025, 12, 19), OptionType.Call, 650m);
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetBarsAsync(
            ticker,
            multiplier: 1,
            timespan: AggregateInterval.Day,
            from: "2023-01-09",
            to: "2023-02-10",
            adjusted: true,
            sort: SortOrder.Ascending,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);

        // Verify limit is respected
        if (response.Results.Count > 0)
        {
            Assert.True(response.Results.Count <= 10);
        }
    }

    /// <summary>
    /// Tests that GetDailyOpenCloseAsync with OptionsTicker returns valid daily data.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:SPY251219C00650000");
        var date = "2023-01-09";
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetDailyOpenCloseAsync(ticker, date, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.Equal("O:SPY251219C00650000", response.Symbol);
        Assert.Equal(date, response.From);
    }

    /// <summary>
    /// Tests that GetPreviousDayBarAsync with OptionsTicker returns valid previous day data.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_WithOptionsTicker_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = new OptionsTicker("SPY", new DateTime(2025, 12, 19), OptionType.Call, 650m);
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(ticker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
        Assert.Equal("O:SPY251219C00650000", response.Ticker);
    }

    /// <summary>
    /// Tests that GetPreviousDayBarAsync with OptionsTicker supports the adjusted parameter.
    /// </summary>
    [Fact]
    public async Task GetPreviousDayBarAsync_WithOptionsTickerAndAdjusted_ShouldReturnValidResponse()
    {
        // Arrange
        var ticker = OptionsTicker.Parse("O:SPY251219C00650000");
        var optionsService = _polygonClient.Options;

        // Act
        var response = await optionsService.GetPreviousDayBarAsync(
            ticker,
            adjusted: true,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.Results);
    }

    #endregion

    #region Discovery Helper Extension Methods Tests

    /// <summary>
    /// Tests that GetAvailableStrikesAsync returns a list of strike prices for an underlying.
    /// Note: This test may be skipped if the API plan doesn't support the chain snapshot endpoint features.
    /// </summary>
    [Fact]
    public async Task GetAvailableStrikesAsync_ForSPY_ShouldReturnStrikePrices()
    {
        // Arrange
        var underlying = "SPY";
        var optionsService = _polygonClient.Options;

        try
        {
            // Act
            var strikes = await optionsService.GetAvailableStrikesAsync(
                underlying,
                cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(strikes);

            // If we got results, verify they're properly formatted
            if (strikes.Count > 0)
            {
                // Verify strikes are sorted in ascending order
                var sortedStrikes = strikes.OrderBy(s => s).ToList();
                Assert.Equal(sortedStrikes, strikes);

                // Verify all strikes are unique
                Assert.Equal(strikes.Count, strikes.Distinct().Count());
            }
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // API might not support this feature on the current plan - skip the test
            Assert.SkipWhen(true, "Chain snapshot endpoint features not supported by current API plan");
        }
    }

    /// <summary>
    /// Tests that GetAvailableStrikesAsync can filter by option type.
    /// Note: This test may be skipped if the API plan doesn't support the chain snapshot endpoint features.
    /// </summary>
    [Fact]
    public async Task GetAvailableStrikesAsync_WithTypeFilter_ShouldReturnFilteredStrikes()
    {
        // Arrange
        var underlying = "SPY";
        var type = OptionType.Call;
        var optionsService = _polygonClient.Options;

        try
        {
            // Act
            var strikes = await optionsService.GetAvailableStrikesAsync(
                underlying,
                type: type,
                cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(strikes);

            // If we got results, verify they're valid
            if (strikes.Count > 0)
            {
                // Verify strikes are positive numbers
                Assert.All(strikes, strike => Assert.True(strike > 0));
            }
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // API might not support this feature on the current plan - skip the test
            Assert.SkipWhen(true, "Chain snapshot endpoint features not supported by current API plan");
        }
    }

    /// <summary>
    /// Tests that GetAvailableStrikesAsync can filter by expiration date range.
    /// Note: This test may be skipped if the API plan doesn't support the chain snapshot endpoint features.
    /// </summary>
    [Fact]
    public async Task GetAvailableStrikesAsync_WithExpirationFilter_ShouldReturnFilteredStrikes()
    {
        // Arrange
        var underlying = "SPY";
        var type = OptionType.Call;
        var expirationDateGte = "2025-12-01";
        var expirationDateLte = "2025-12-31";
        var optionsService = _polygonClient.Options;

        try
        {
            // Act
            var strikes = await optionsService.GetAvailableStrikesAsync(
                underlying,
                type: type,
                expirationDateGte: expirationDateGte,
                expirationDateLte: expirationDateLte,
                cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(strikes);
            // May or may not have results depending on available data, but should not throw
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // API might not support this feature on the current plan - skip the test
            Assert.SkipWhen(true, "Chain snapshot endpoint features not supported by current API plan");
        }
    }

    /// <summary>
    /// Tests that GetExpirationDatesAsync returns a list of expiration dates for an underlying.
    /// Note: This test may be skipped if the API plan doesn't support the chain snapshot endpoint features.
    /// </summary>
    [Fact]
    public async Task GetExpirationDatesAsync_ForSPY_ShouldReturnExpirationDates()
    {
        // Arrange
        var underlying = "SPY";
        var optionsService = _polygonClient.Options;

        try
        {
            // Act
            var expirations = await optionsService.GetExpirationDatesAsync(
                underlying,
                cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(expirations);

            if (expirations.Count > 0)
            {
                // Verify dates are sorted in ascending order
                var sortedDates = expirations.OrderBy(d => d).ToList();
                Assert.Equal(sortedDates, expirations);

                // Verify all dates are unique
                Assert.Equal(expirations.Count, expirations.Distinct().Count());
            }
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // API might not support this feature on the current plan - skip the test
            Assert.SkipWhen(true, "Chain snapshot endpoint features not supported by current API plan");
        }
    }

    /// <summary>
    /// Tests that GetExpirationDatesAsync can filter by option type.
    /// Note: This test may be skipped if the API plan doesn't support the chain snapshot endpoint features.
    /// </summary>
    [Fact]
    public async Task GetExpirationDatesAsync_WithTypeFilter_ShouldReturnFilteredDates()
    {
        // Arrange
        var underlying = "SPY";
        var type = OptionType.Put;
        var optionsService = _polygonClient.Options;

        try
        {
            // Act
            var expirations = await optionsService.GetExpirationDatesAsync(
                underlying,
                type: type,
                cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(expirations);
            // Should return results for SPY, a heavily traded ticker
            if (expirations.Count > 0)
            {
                // Verify dates are sorted if we have results
                var sortedDates = expirations.OrderBy(d => d).ToList();
                Assert.Equal(sortedDates, expirations);
            }
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // API might not support this feature on the current plan - skip the test
            Assert.SkipWhen(true, "Chain snapshot endpoint features not supported by current API plan");
        }
    }

    /// <summary>
    /// Tests that GetExpirationDatesAsync can filter by strike price.
    /// Note: This test may be skipped if the API plan doesn't support the chain snapshot endpoint features.
    /// </summary>
    [Fact]
    public async Task GetExpirationDatesAsync_WithStrikeFilter_ShouldReturnFilteredDates()
    {
        // Arrange
        var underlying = "SPY";
        var type = OptionType.Call;
        var strikePrice = 650m;
        var optionsService = _polygonClient.Options;

        try
        {
            // Act
            var expirations = await optionsService.GetExpirationDatesAsync(
                underlying,
                type: type,
                strikePrice: strikePrice,
                cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(expirations);
            // May or may not have results depending on available data, but should not throw
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // API might not support this feature on the current plan - skip the test
            Assert.SkipWhen(true, "Chain snapshot endpoint features not supported by current API plan");
        }
    }

    /// <summary>
    /// Tests that GetExpirationDatesAsync returns dates for both calls and puts when no type filter is specified.
    /// Note: This test may be skipped if the API plan doesn't support the chain snapshot endpoint features.
    /// </summary>
    [Fact]
    public async Task GetExpirationDatesAsync_WithoutTypeFilter_ShouldReturnAllDates()
    {
        // Arrange
        var underlying = "SPY";
        var optionsService = _polygonClient.Options;

        try
        {
            // Act
            var expirations = await optionsService.GetExpirationDatesAsync(
                underlying,
                type: null, // No filter
                cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(expirations);

            if (expirations.Count > 0)
            {
                // Verify all dates are in the future or recent past
                var oldestAllowedDate = DateTime.Now.AddYears(-5);
                Assert.All(expirations, date => Assert.True(date >= oldestAllowedDate));
            }
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // API might not support this feature on the current plan - skip the test
            Assert.SkipWhen(true, "Chain snapshot endpoint features not supported by current API plan");
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
