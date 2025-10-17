// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;

namespace TreyThomasCodes.Polygon.IntegrationTests.ReferenceData;

/// <summary>
/// Integration tests for fetching condition codes data from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetConditionCodesIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching condition codes from Polygon.io with stocks asset class filter.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetConditionCodesAsync_WithStocksFilter_ShouldReturnValidResponse()
    {
        // Arrange
        var referenceDataService = PolygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: AssetClass.Stocks,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(conditionCodesResponse);
        Assert.Equal("OK", conditionCodesResponse.Status);
        Assert.NotEmpty(conditionCodesResponse.RequestId);
        Assert.True(conditionCodesResponse.Count > 0);
        Assert.NotNull(conditionCodesResponse.Results);
        Assert.True(conditionCodesResponse.Results.Count > 0);
    }

    /// <summary>
    /// Tests that the condition codes data structure is properly typed.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetConditionCodesAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var referenceDataService = PolygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: AssetClass.Stocks,
            limit: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(conditionCodesResponse);
        Assert.IsType<List<ConditionCode>>(conditionCodesResponse.Results);
        Assert.IsType<int>(conditionCodesResponse.Count);
        Assert.IsType<string>(conditionCodesResponse.Status);
        Assert.IsType<string>(conditionCodesResponse.RequestId);

        // Verify at least one condition code has proper structure
        var firstCode = conditionCodesResponse.Results.First();
        Assert.IsType<int>(firstCode.Id);
        Assert.IsType<string>(firstCode.Type);
        Assert.IsType<string>(firstCode.Name);
        Assert.IsType<string>(firstCode.AssetClass);
        Assert.IsType<SipMapping>(firstCode.SipMapping);
        Assert.IsType<UpdateRules>(firstCode.UpdateRules);
        Assert.IsType<List<string>>(firstCode.DataTypes);
    }
}
