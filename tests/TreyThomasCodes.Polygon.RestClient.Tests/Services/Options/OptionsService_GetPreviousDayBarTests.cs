// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services.Options;

/// <summary>
/// Unit tests for the OptionsService.GetPreviousDayBarAsync method.
/// Tests the service's ability to retrieve data from Polygon.io API.
/// </summary>
public class OptionsService_GetPreviousDayBarTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetPreviousDayBarTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public OptionsService_GetPreviousDayBarTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _service = new OptionsService(_mockApi.Object);
    }


/// <summary>
/// Tests that GetPreviousDayBarAsync calls the API and returns the previous day bar response.
/// </summary>
[Fact]
public async Task GetPreviousDayBarAsync_CallsApi_ReturnsPreviousDayBarResponse()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var expectedResponse = new PolygonResponse<List<OptionBar>>
    {
        Ticker = optionsTicker,
        QueryCount = 1,
        ResultsCount = 1,
        Adjusted = true,
        Results = new List<OptionBar>
        {
            new OptionBar
            {
                Volume = 568,
                VolumeWeightedAveragePrice = 29.4581m,
                Open = 28.79m,
                Close = 29.97m,
                High = 31.03m,
                Low = 28.36m,
                Timestamp = 1760385600000,
                NumberOfTransactions = 155
            }
        },
        Status = "OK",
        RequestId = "ccf7f0778ccf8a800e1977ba15c8410a",
        Count = 1
    };

    _mockApi
        .Setup(x => x.GetPreviousDayBarAsync(
            optionsTicker,
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetPreviousDayBarAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedResponse.Ticker, result.Ticker);
    Assert.Equal(expectedResponse.QueryCount, result.QueryCount);
    Assert.Equal(expectedResponse.ResultsCount, result.ResultsCount);
    Assert.Equal(expectedResponse.Adjusted, result.Adjusted);
    Assert.Equal(expectedResponse.Status, result.Status);
    Assert.Equal(expectedResponse.Count, result.Count);
    Assert.NotNull(result.Results);
    Assert.Single(result.Results);

    var bar = result.Results[0];
    Assert.Equal(568ul, bar.Volume);
    Assert.Equal(29.4581m, bar.VolumeWeightedAveragePrice);
    Assert.Equal(28.79m, bar.Open);
    Assert.Equal(29.97m, bar.Close);
    Assert.Equal(31.03m, bar.High);
    Assert.Equal(28.36m, bar.Low);
    Assert.Equal(1760385600000ul, bar.Timestamp);
    Assert.Equal(155, bar.NumberOfTransactions);

    _mockApi.Verify(x => x.GetPreviousDayBarAsync(
        optionsTicker,
        It.IsAny<bool?>(),
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetPreviousDayBarAsync passes parameters correctly to the API.
/// </summary>
[Fact]
public async Task GetPreviousDayBarAsync_PassesParametersToApi()
{
    // Arrange
    var optionsTicker = "O:TSLA260320C00700000";
    var adjusted = false;
    var expectedResponse = new PolygonResponse<List<OptionBar>>
    {
        Results = new List<OptionBar>()
    };

    _mockApi
        .Setup(x => x.GetPreviousDayBarAsync(
            optionsTicker,
            adjusted,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    await _service.GetPreviousDayBarAsync(optionsTicker, adjusted, TestContext.Current.CancellationToken);

    // Assert
    _mockApi.Verify(x => x.GetPreviousDayBarAsync(
        optionsTicker,
        adjusted,
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetPreviousDayBarAsync handles null values correctly.
/// </summary>
[Fact]
public async Task GetPreviousDayBarAsync_HandlesNullValues()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var expectedResponse = new PolygonResponse<List<OptionBar>>
    {
        Ticker = optionsTicker,
        QueryCount = 1,
        ResultsCount = 1,
        Adjusted = true,
        Results = new List<OptionBar>
        {
            new OptionBar
            {
                Volume = null,
                VolumeWeightedAveragePrice = null,
                Open = null,
                Close = null,
                High = null,
                Low = null,
                Timestamp = null,
                NumberOfTransactions = null
            }
        },
        Status = "OK"
    };

    _mockApi
        .Setup(x => x.GetPreviousDayBarAsync(
            optionsTicker,
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetPreviousDayBarAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Results);
    Assert.Single(result.Results);

    var bar = result.Results[0];
    Assert.Null(bar.Volume);
    Assert.Null(bar.VolumeWeightedAveragePrice);
    Assert.Null(bar.Open);
    Assert.Null(bar.Close);
    Assert.Null(bar.High);
    Assert.Null(bar.Low);
    Assert.Null(bar.Timestamp);
    Assert.Null(bar.NumberOfTransactions);
}

/// <summary>
/// Tests that GetPreviousDayBarAsync returns empty results list when no data is available.
/// </summary>
[Fact]
public async Task GetPreviousDayBarAsync_HandlesEmptyResults()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var expectedResponse = new PolygonResponse<List<OptionBar>>
    {
        Ticker = optionsTicker,
        QueryCount = 0,
        ResultsCount = 0,
        Adjusted = true,
        Results = new List<OptionBar>(),
        Status = "OK"
    };

    _mockApi
        .Setup(x => x.GetPreviousDayBarAsync(
            optionsTicker,
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetPreviousDayBarAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Results);
    Assert.Empty(result.Results);
    Assert.Equal(0, result.ResultsCount);
}
}
