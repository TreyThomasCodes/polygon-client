// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

namespace TreyThomasCodes.Polygon.IntegrationTests.ReferenceData;

/// <summary>
/// Integration tests for fetching market status data from Polygon.io.
/// These tests require a valid Polygon API key to be configured in user secrets.
/// </summary>
public class GetMarketStatusIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching current market status from Polygon.io.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_ShouldReturnValidResponse()
    {
        // Arrange
        var referenceDataService = PolygonClient.ReferenceData;
        var request = new GetMarketStatusRequest();

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(
            request,
            TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(marketStatus);
        Assert.NotNull(marketStatus.Market);
        Assert.NotEmpty(marketStatus.Market);
        Assert.NotNull(marketStatus.ServerTime);
        Assert.NotEmpty(marketStatus.ServerTime);
    }

    /// <summary>
    /// Tests that the market status data structure is properly typed.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var referenceDataService = PolygonClient.ReferenceData;
        var request = new GetMarketStatusRequest();

        // Act
        var marketStatus = await referenceDataService.GetMarketStatusAsync(
            request,
            TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(marketStatus);
        Assert.IsType<MarketStatus>(marketStatus);

        // Verify property types
        Assert.IsType<string>(marketStatus.Market);
        Assert.IsType<bool>(marketStatus.AfterHours);
        Assert.IsType<bool>(marketStatus.EarlyHours);
        Assert.IsType<string>(marketStatus.ServerTime);

        // Verify nested object types if present
        if (marketStatus.Exchanges != null)
        {
            Assert.IsType<ExchangesStatus>(marketStatus.Exchanges);
        }

        if (marketStatus.Currencies != null)
        {
            Assert.IsType<CurrencyMarketsStatus>(marketStatus.Currencies);
        }

        if (marketStatus.IndicesGroups != null)
        {
            Assert.IsType<IndicesGroupsStatus>(marketStatus.IndicesGroups);
        }
    }
}
