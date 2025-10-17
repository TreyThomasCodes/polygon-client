// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Reference;

namespace TreyThomasCodes.Polygon.IntegrationTests.ReferenceData;

/// <summary>
/// Integration tests for fetching ticker types data from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetTickerTypesIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching ticker types from Polygon.io.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetTickerTypesAsync_ShouldReturnValidResponse()
    {
        // Arrange
        var referenceDataService = PolygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(tickerTypesResponse);
        Assert.Equal("OK", tickerTypesResponse.Status);
        Assert.NotEmpty(tickerTypesResponse.RequestId);
        Assert.True(tickerTypesResponse.Count > 0);
        Assert.NotNull(tickerTypesResponse.Results);
        Assert.True(tickerTypesResponse.Results.Count > 0);
        Assert.Equal(tickerTypesResponse.Count, tickerTypesResponse.Results.Count);
    }

    /// <summary>
    /// Tests that the ticker types data structure is properly typed.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetTickerTypesAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var referenceDataService = PolygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(tickerTypesResponse);
        Assert.IsType<TickerTypesResponse>(tickerTypesResponse);

        // Verify property types
        Assert.IsType<List<TickerType>>(tickerTypesResponse.Results);
        Assert.IsType<int>(tickerTypesResponse.Count);
        Assert.IsType<string>(tickerTypesResponse.Status);
        Assert.IsType<string>(tickerTypesResponse.RequestId);

        // Verify at least one ticker type has proper structure
        var firstType = tickerTypesResponse.Results.First();
        Assert.IsType<string>(firstType.Code);
        Assert.IsType<string>(firstType.Description);
        Assert.IsType<string>(firstType.AssetClass);
        Assert.IsType<string>(firstType.Locale);
    }
}
