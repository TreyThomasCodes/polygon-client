// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.IntegrationTests.Options;

/// <summary>
/// Integration tests for fetching options chain snapshot data from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetChainSnapshotIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching option chain snapshot for SPY.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_ForSPY_ShouldReturnValidResponse()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionsService = PolygonClient.Options;

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
        var optionsService = PolygonClient.Options;

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
}
