// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services.Options;

/// <summary>
/// Unit tests for the OptionsService.GetDailyOpenCloseAsync method.
/// Tests the service's ability to retrieve data from Polygon.io API.
/// </summary>
public class OptionsService_GetDailyOpenCloseTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetDailyOpenCloseTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public OptionsService_GetDailyOpenCloseTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _service = new OptionsService(_mockApi.Object);
    }


/// <summary>
/// Tests that GetDailyOpenCloseAsync calls the API and returns the daily open/close response.
/// </summary>
[Fact]
public async Task GetDailyOpenCloseAsync_CallsApi_ReturnsDailyOpenCloseResponse()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var date = "2023-01-09";
    var expectedResponse = new OptionDailyOpenClose
    {
        Status = "OK",
        From = "2023-01-09",
        Symbol = "O:SPY251219C00650000",
        Open = 4.08m,
        High = 4.08m,
        Low = 3.89m,
        Close = 3.89m,
        Volume = 2,
        AfterHours = 3.89m,
        PreMarket = 4.08m
    };

    _mockApi
        .Setup(x => x.GetDailyOpenCloseAsync(
            optionsTicker,
            date,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetDailyOpenCloseAsync(optionsTicker, date, TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedResponse.Status, result.Status);
    Assert.Equal(expectedResponse.From, result.From);
    Assert.Equal(expectedResponse.Symbol, result.Symbol);
    Assert.Equal(expectedResponse.Open, result.Open);
    Assert.Equal(expectedResponse.High, result.High);
    Assert.Equal(expectedResponse.Low, result.Low);
    Assert.Equal(expectedResponse.Close, result.Close);
    Assert.Equal(expectedResponse.Volume, result.Volume);
    Assert.Equal(expectedResponse.AfterHours, result.AfterHours);
    Assert.Equal(expectedResponse.PreMarket, result.PreMarket);

    _mockApi.Verify(x => x.GetDailyOpenCloseAsync(
        optionsTicker,
        date,
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetDailyOpenCloseAsync passes parameters correctly to the API.
/// </summary>
[Fact]
public async Task GetDailyOpenCloseAsync_PassesParametersToApi()
{
    // Arrange
    var optionsTicker = "O:TSLA260320C00700000";
    var date = "2023-03-20";
    var expectedResponse = new OptionDailyOpenClose();

    _mockApi
        .Setup(x => x.GetDailyOpenCloseAsync(
            optionsTicker,
            date,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    await _service.GetDailyOpenCloseAsync(optionsTicker, date, TestContext.Current.CancellationToken);

    // Assert
    _mockApi.Verify(x => x.GetDailyOpenCloseAsync(
        optionsTicker,
        date,
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetDailyOpenCloseAsync handles null values correctly.
/// </summary>
[Fact]
public async Task GetDailyOpenCloseAsync_HandlesNullValues()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var date = "2023-01-09";
    var expectedResponse = new OptionDailyOpenClose
    {
        Status = "OK",
        From = "2023-01-09",
        Symbol = "O:SPY251219C00650000",
        Open = null,
        High = null,
        Low = null,
        Close = null,
        Volume = null,
        AfterHours = null,
        PreMarket = null
    };

    _mockApi
        .Setup(x => x.GetDailyOpenCloseAsync(
            optionsTicker,
            date,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetDailyOpenCloseAsync(optionsTicker, date, TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.Null(result.Open);
    Assert.Null(result.High);
    Assert.Null(result.Low);
    Assert.Null(result.Close);
    Assert.Null(result.Volume);
    Assert.Null(result.AfterHours);
    Assert.Null(result.PreMarket);
}


}
