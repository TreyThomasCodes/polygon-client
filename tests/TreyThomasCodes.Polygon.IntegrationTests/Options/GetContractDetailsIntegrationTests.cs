// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.IntegrationTests.Options;

/// <summary>
/// Integration tests for fetching options contract details from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class GetContractDetailsIntegrationTests : IntegrationTestBase
{
    /// <summary>
    /// Tests fetching SPY call option contract details.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ForSPYCallOption_ShouldReturnValidResponse()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var optionsService = PolygonClient.Options;

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
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var optionsService = PolygonClient.Options;

        // Act
        var response = await optionsService.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<Models.Options.OptionsContract>>(response);

        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<Models.Options.OptionsContract>(response.Results);
    }

    /// <summary>
    /// Tests that the client correctly handles errors for invalid options tickers.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "O:INVALID000000C00000000";
        var optionsService = PolygonClient.Options;

        // Act & Assert - Verify client properly handles API errors
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => optionsService.GetContractDetailsAsync(invalidTicker, TestContext.Current.CancellationToken));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }
}
