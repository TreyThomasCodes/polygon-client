// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;

namespace TreyThomasCodes.Polygon.IntegrationTests.ReferenceData;

/// <summary>
/// Integration tests for fetching exchanges data from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetExchangesIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching exchanges from Polygon.io with stocks asset class filter.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetExchangesAsync_WithStocksFilter_ShouldReturnValidResponse()
    {
        // Arrange
        var referenceDataService = PolygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: AssetClass.Stocks,
            locale: Locale.UnitedStates,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(exchangesResponse);
        Assert.Equal("OK", exchangesResponse.Status);
        Assert.NotEmpty(exchangesResponse.RequestId);
        Assert.True(exchangesResponse.Count > 0);
        Assert.NotNull(exchangesResponse.Results);
        Assert.True(exchangesResponse.Results.Count > 0);
    }

    /// <summary>
    /// Tests that the exchanges data structure is properly typed.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetExchangesAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var referenceDataService = PolygonClient.ReferenceData;

        // Act
        var exchangesResponse = await referenceDataService.GetExchangesAsync(
            assetClass: AssetClass.Stocks,
            locale: Locale.UnitedStates,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(exchangesResponse);
        Assert.IsType<List<Exchange>>(exchangesResponse.Results);
        Assert.IsType<int>(exchangesResponse.Count);
        Assert.IsType<string>(exchangesResponse.Status);
        Assert.IsType<string>(exchangesResponse.RequestId);

        // Verify at least one exchange has proper structure
        var firstExchange = exchangesResponse.Results.First();
        Assert.IsType<int>(firstExchange.Id);
        Assert.IsType<string>(firstExchange.Type);
        Assert.IsType<string>(firstExchange.AssetClass);
        Assert.IsType<string>(firstExchange.Locale);
        Assert.IsType<string>(firstExchange.Name);
        Assert.IsType<string>(firstExchange.OperatingMic);
    }
}
