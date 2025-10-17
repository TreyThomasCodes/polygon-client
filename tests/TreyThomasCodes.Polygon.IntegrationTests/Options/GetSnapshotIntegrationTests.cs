// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.IntegrationTests.Options;

/// <summary>
/// Integration tests for fetching options snapshot data from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetSnapshotIntegrationTests : IntegrationTestBase
{
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
        var optionsService = PolygonClient.Options;

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
        var optionsService = PolygonClient.Options;

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
        var optionsService = PolygonClient.Options;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => optionsService.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }
}
