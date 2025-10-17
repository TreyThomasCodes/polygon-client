// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services.Options;

/// <summary>
/// Unit tests for the OptionsService.GetBarsAsync method.
/// Tests the service's ability to retrieve data from Polygon.io API.
/// </summary>
public class OptionsService_GetBarsTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetBarsTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public OptionsService_GetBarsTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _service = new OptionsService(_mockApi.Object);
    }


/// <summary>
/// Tests that GetBarsAsync calls the API and returns the bars response.
/// </summary>
[Fact]
public async Task GetBarsAsync_CallsApi_ReturnsBarsResponse()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var multiplier = 1;
    var timespan = AggregateInterval.Day;
    var from = "2023-01-09";
    var to = "2023-02-10";
    var expectedResponse = new PolygonResponse<List<OptionBar>>
    {
        Ticker = optionsTicker,
        QueryCount = 23,
        ResultsCount = 23,
        Adjusted = true,
        Results = new List<OptionBar>
        {
            new OptionBar
            {
                Volume = 2,
                VolumeWeightedAveragePrice = 3.985m,
                Open = 4.08m,
                Close = 3.89m,
                High = 4.08m,
                Low = 3.89m,
                Timestamp = 1673240400000,
                NumberOfTransactions = 2
            },
            new OptionBar
            {
                Volume = 3,
                VolumeWeightedAveragePrice = 3.9m,
                Open = 3.9m,
                Close = 3.9m,
                High = 3.9m,
                Low = 3.9m,
                Timestamp = 1673326800000,
                NumberOfTransactions = 1
            }
        },
        Status = "OK",
        RequestId = "32ce135d201dc77533d486784681bd2e",
        Count = 23
    };

    _mockApi.Setup(x => x.GetBarsAsync(
        optionsTicker,
        multiplier,
        timespan,
        from,
        to,
        It.IsAny<bool?>(),
        It.IsAny<SortOrder?>(),
        It.IsAny<int?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedResponse.Status, result.Status);
    Assert.Equal(expectedResponse.RequestId, result.RequestId);
    Assert.Equal(expectedResponse.Ticker, result.Ticker);
    Assert.Equal(expectedResponse.QueryCount, result.QueryCount);
    Assert.Equal(expectedResponse.ResultsCount, result.ResultsCount);
    Assert.Equal(expectedResponse.Adjusted, result.Adjusted);
    Assert.Equal(expectedResponse.Count, result.Count);
    Assert.NotNull(result.Results);
    Assert.Equal(2, result.Results.Count);

    var firstBar = result.Results[0];
    Assert.Equal(2ul, firstBar.Volume);
    Assert.Equal(3.985m, firstBar.VolumeWeightedAveragePrice);
    Assert.Equal(4.08m, firstBar.Open);
    Assert.Equal(3.89m, firstBar.Close);
    Assert.Equal(4.08m, firstBar.High);
    Assert.Equal(3.89m, firstBar.Low);
    Assert.Equal(1673240400000ul, firstBar.Timestamp);
    Assert.Equal(2, firstBar.NumberOfTransactions);

    _mockApi.Verify(x => x.GetBarsAsync(
        optionsTicker,
        multiplier,
        timespan,
        from,
        to,
        It.IsAny<bool?>(),
        It.IsAny<SortOrder?>(),
        It.IsAny<int?>(),
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetBarsAsync passes the cancellation token to the API.
/// </summary>
[Fact]
public async Task GetBarsAsync_PassesCancellationToken()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var multiplier = 1;
    var timespan = AggregateInterval.Day;
    var from = "2023-01-09";
    var to = "2023-02-10";
    var cancellationToken = new CancellationToken();
    var expectedResponse = new PolygonResponse<List<OptionBar>> { Status = "OK" };

    _mockApi.Setup(x => x.GetBarsAsync(
        optionsTicker,
        multiplier,
        timespan,
        from,
        to,
        It.IsAny<bool?>(),
        It.IsAny<SortOrder?>(),
        It.IsAny<int?>(),
        cancellationToken))
        .ReturnsAsync(expectedResponse);

    // Act
    await _service.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: cancellationToken);

    // Assert
    _mockApi.Verify(x => x.GetBarsAsync(
        optionsTicker,
        multiplier,
        timespan,
        from,
        to,
        It.IsAny<bool?>(),
        It.IsAny<SortOrder?>(),
        It.IsAny<int?>(),
        cancellationToken), Times.Once);
}

/// <summary>
/// Tests that GetBarsAsync passes all optional parameters to the API.
/// </summary>
[Fact]
public async Task GetBarsAsync_PassesOptionalParameters()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var multiplier = 5;
    var timespan = AggregateInterval.Minute;
    var from = "2023-01-09";
    var to = "2023-01-10";
    var adjusted = true;
    var sort = SortOrder.Ascending;
    var limit = 100;
    var expectedResponse = new PolygonResponse<List<OptionBar>> { Status = "OK" };

    _mockApi.Setup(x => x.GetBarsAsync(
        optionsTicker,
        multiplier,
        timespan,
        from,
        to,
        adjusted,
        sort,
        limit,
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    await _service.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, adjusted, sort, limit, TestContext.Current.CancellationToken);

    // Assert
    _mockApi.Verify(x => x.GetBarsAsync(
        optionsTicker,
        multiplier,
        timespan,
        from,
        to,
        adjusted,
        sort,
        limit,
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetBarsAsync MarketTimestamp property converts timestamp correctly.
/// </summary>
[Fact]
public async Task GetBarsAsync_MarketTimestamp_ConvertsToEasternTime()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var multiplier = 1;
    var timespan = AggregateInterval.Day;
    var from = "2023-01-09";
    var to = "2023-02-10";
    var timestamp = 1673240400000ul;
    var expectedResponse = new PolygonResponse<List<OptionBar>>
    {
        Results = new List<OptionBar>
        {
            new OptionBar
            {
                Volume = 2,
                VolumeWeightedAveragePrice = 3.985m,
                Open = 4.08m,
                Close = 3.89m,
                High = 4.08m,
                Low = 3.89m,
                Timestamp = timestamp,
                NumberOfTransactions = 2
            }
        },
        Status = "OK"
    };

    _mockApi.Setup(x => x.GetBarsAsync(
        optionsTicker,
        multiplier,
        timespan,
        from,
        to,
        It.IsAny<bool?>(),
        It.IsAny<SortOrder?>(),
        It.IsAny<int?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Results);
    Assert.Single(result.Results);
    Assert.Equal(timestamp, result.Results[0].Timestamp);
    Assert.NotNull(result.Results[0].MarketTimestamp);
    Assert.Equal("America/New_York", result.Results[0].MarketTimestamp.Value.Zone.Id);
}

/// <summary>
/// Tests that GetBarsAsync handles null timestamp in MarketTimestamp property.
/// </summary>
[Fact]
public async Task GetBarsAsync_MarketTimestamp_WhenTimestampIsNull_ReturnsNull()
{
    // Arrange
    var optionsTicker = "O:SPY251219C00650000";
    var multiplier = 1;
    var timespan = AggregateInterval.Day;
    var from = "2023-01-09";
    var to = "2023-02-10";
    var expectedResponse = new PolygonResponse<List<OptionBar>>
    {
        Results = new List<OptionBar>
        {
            new OptionBar
            {
                Volume = 2,
                VolumeWeightedAveragePrice = 3.985m,
                Open = 4.08m,
                Close = 3.89m,
                High = 4.08m,
                Low = 3.89m,
                Timestamp = null,
                NumberOfTransactions = 2
            }
        },
        Status = "OK"
    };

    _mockApi.Setup(x => x.GetBarsAsync(
        optionsTicker,
        multiplier,
        timespan,
        from,
        to,
        It.IsAny<bool?>(),
        It.IsAny<SortOrder?>(),
        It.IsAny<int?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetBarsAsync(optionsTicker, multiplier, timespan, from, to, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Results);
    Assert.Single(result.Results);
    Assert.Null(result.Results[0].Timestamp);
    Assert.Null(result.Results[0].MarketTimestamp);
}


}
