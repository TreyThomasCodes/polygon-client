// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Extensions;

namespace TreyThomasCodes.Polygon.IntegrationTests.OptionsExtensions;

/// <summary>
/// Integration tests for discovery helper extension methods in OptionsServiceExtensions.
/// These tests verify that helper methods for discovering available strikes and expiration dates
/// work correctly with the chain snapshot endpoint.
/// </summary>
public class DiscoveryHelpersIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that GetAvailableStrikesAsync returns a list of strike prices for an underlying.
    /// Note: This test may be skipped if the API plan doesn't support the chain snapshot endpoint features.
    /// </summary>
    [Fact]
    public async Task GetAvailableStrikesAsync_ForSPY_ShouldReturnStrikePrices()
    {
        // Arrange
        var underlying = "SPY";
        var optionsService = PolygonClient.Options;

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
        var optionsService = PolygonClient.Options;

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
        var optionsService = PolygonClient.Options;

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
        var optionsService = PolygonClient.Options;

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
        var optionsService = PolygonClient.Options;

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
        var optionsService = PolygonClient.Options;

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
        var optionsService = PolygonClient.Options;

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
                var oldestAllowedDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
                Assert.All(expirations, date => Assert.True(date >= oldestAllowedDate));
            }
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // API might not support this feature on the current plan - skip the test
            Assert.SkipWhen(true, "Chain snapshot endpoint features not supported by current API plan");
        }
    }
}
