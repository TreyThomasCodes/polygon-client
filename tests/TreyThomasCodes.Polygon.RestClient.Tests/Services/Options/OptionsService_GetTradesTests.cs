// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services.Options;

/// <summary>
/// Unit tests for the OptionsService.GetTradesAsync method.
/// Tests the service's ability to retrieve data from Polygon.io API.
/// </summary>
public class OptionsService_GetTradesTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetTradesTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public OptionsService_GetTradesTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _service = new OptionsService(_mockApi.Object);
    }


/// <summary>
/// Tests that GetTradesAsync calls the API and returns the trades response.
/// </summary>
[Fact]
public async Task GetTradesAsync_CallsApi_ReturnsTradesResponse()
{
    // Arrange
    var optionsTicker = "O:TSLA210903C00700000";
    var expectedResponse = new PolygonResponse<List<OptionTradeV3>>
    {
        Results = new List<OptionTradeV3>
        {
            new OptionTradeV3
            {
                Conditions = new List<int> { 209 },
                Exchange = 303,
                Id = "",
                ParticipantTimestamp = 1626965921018000000,
                Price = 25m,
                SequenceNumber = 0,
                SipTimestamp = 1626965921018000000,
                Size = 1
            },
            new OptionTradeV3
            {
                Conditions = new List<int> { 232 },
                Exchange = 312,
                Id = "",
                ParticipantTimestamp = 1626978552757000000,
                Price = 26.2m,
                SequenceNumber = 0,
                SipTimestamp = 1626978552757000000,
                Size = 1
            }
        },
        Status = "OK",
        RequestId = "9791c1dc5e696cd5b3f085f252156918",
        NextUrl = "https://api.polygon.io/v3/trades/O:TSLA210903C00700000?cursor=test"
    };

    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetTradesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedResponse.Status, result.Status);
    Assert.Equal(expectedResponse.RequestId, result.RequestId);
    Assert.Equal(expectedResponse.NextUrl, result.NextUrl);
    Assert.NotNull(result.Results);
    Assert.Equal(2, result.Results.Count);
    Assert.Equal(25m, result.Results[0].Price);
    Assert.Equal(303, result.Results[0].Exchange);
    Assert.Equal(1, result.Results[0].Size);

    _mockApi.Verify(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetTradesAsync passes the cancellation token to the API.
/// </summary>
[Fact]
public async Task GetTradesAsync_PassesCancellationToken()
{
    // Arrange
    var optionsTicker = "O:TSLA210903C00700000";
    var cancellationToken = new CancellationToken();
    var expectedResponse = new PolygonResponse<List<OptionTradeV3>> { Status = "OK" };

    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        cancellationToken))
        .ReturnsAsync(expectedResponse);

    // Act
    await _service.GetTradesAsync(optionsTicker, cancellationToken: cancellationToken);

    // Assert
    _mockApi.Verify(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        cancellationToken), Times.Once);
}

/// <summary>
/// Tests that GetTradesAsync passes all parameters correctly to the API.
/// </summary>
[Fact]
public async Task GetTradesAsync_WithAllParameters_PassesParametersCorrectly()
{
    // Arrange
    var optionsTicker = "O:TSLA210903C00700000";
    var timestamp = "2021-07-23";
    var timestampLt = "2021-07-24";
    var timestampLte = "2021-07-23T23:59:59";
    var timestampGt = "2021-07-22";
    var timestampGte = "2021-07-23T00:00:00";
    var order = "asc";
    var limit = 10;
    var sort = "timestamp";
    var cursor = "test-cursor";
    var expectedResponse = new PolygonResponse<List<OptionTradeV3>>
    {
        Results = new List<OptionTradeV3>(),
        Status = "OK",
        RequestId = "test-request-id"
    };

    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        timestamp,
        timestampLt,
        timestampLte,
        timestampGt,
        timestampGte,
        order,
        limit,
        sort,
        cursor,
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetTradesAsync(
        optionsTicker,
        timestamp,
        timestampLt,
        timestampLte,
        timestampGt,
        timestampGte,
        order,
        limit,
        sort,
        cursor,
        TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedResponse.Status, result.Status);
    Assert.Equal(expectedResponse.RequestId, result.RequestId);
    _mockApi.Verify(x => x.GetTradesAsync(
        optionsTicker,
        timestamp,
        timestampLt,
        timestampLte,
        timestampGt,
        timestampGte,
        order,
        limit,
        sort,
        cursor,
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetTradesAsync handles different options ticker symbols correctly.
/// </summary>
[Theory]
[InlineData("O:TSLA210903C00700000")]
[InlineData("O:SPY241220P00720000")]
[InlineData("O:AAPL250117C00150000")]
[InlineData("O:MSFT250321C00400000")]
public async Task GetTradesAsync_WithDifferentTickers_CallsApiWithCorrectTicker(string optionsTicker)
{
    // Arrange
    var expectedResponse = new PolygonResponse<List<OptionTradeV3>>
    {
        Results = new List<OptionTradeV3>
        {
            new OptionTradeV3
            {
                Price = 25m,
                Size = 1,
                Exchange = 303
            }
        },
        Status = "OK"
    };

    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetTradesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    _mockApi.Verify(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetTradesAsync handles null response from API.
/// </summary>
[Fact]
public async Task GetTradesAsync_WhenApiReturnsNull_ReturnsNull()
{
    // Arrange
    var optionsTicker = "O:INVALID000000C00000000";
    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync((PolygonResponse<List<OptionTradeV3>>)null!);

    // Act
    var result = await _service.GetTradesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.Null(result);
    _mockApi.Verify(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()), Times.Once);
}

/// <summary>
/// Tests that GetTradesAsync propagates exceptions from the API.
/// </summary>
[Fact]
public async Task GetTradesAsync_WhenApiThrowsException_PropagatesException()
{
    // Arrange
    var optionsTicker = "O:TSLA210903C00700000";
    var expectedException = new HttpRequestException("Network error");
    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()))
        .ThrowsAsync(expectedException);

    // Act & Assert
    var actualException = await Assert.ThrowsAsync<HttpRequestException>(
        () => _service.GetTradesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken));
    Assert.Equal(expectedException.Message, actualException.Message);
}

/// <summary>
/// Tests that GetTradesAsync handles response with null results.
/// </summary>
[Fact]
public async Task GetTradesAsync_WithNullResults_ReturnsResponseWithNullResults()
{
    // Arrange
    var optionsTicker = "O:TSLA210903C00700000";
    var expectedResponse = new PolygonResponse<List<OptionTradeV3>>
    {
        Results = null,
        Status = "NOT_FOUND",
        RequestId = "test-request-id"
    };

    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetTradesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedResponse.Status, result.Status);
    Assert.Equal(expectedResponse.RequestId, result.RequestId);
    Assert.Null(result.Results);
}

/// <summary>
/// Tests that GetTradesAsync handles empty results list.
/// </summary>
[Fact]
public async Task GetTradesAsync_WithEmptyResults_ReturnsEmptyList()
{
    // Arrange
    var optionsTicker = "O:TSLA210903C00700000";
    var expectedResponse = new PolygonResponse<List<OptionTradeV3>>
    {
        Results = new List<OptionTradeV3>(),
        Status = "OK",
        RequestId = "test-request-id"
    };

    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetTradesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Results);
    Assert.Empty(result.Results);
}

/// <summary>
/// Tests that GetTradesAsync handles pagination with next_url correctly.
/// </summary>
[Fact]
public async Task GetTradesAsync_WithPagination_ReturnsNextUrl()
{
    // Arrange
    var optionsTicker = "O:TSLA210903C00700000";
    var nextUrl = "https://api.polygon.io/v3/trades/O:TSLA210903C00700000?cursor=test-cursor";
    var expectedResponse = new PolygonResponse<List<OptionTradeV3>>
    {
        Results = new List<OptionTradeV3>
        {
            new OptionTradeV3
            {
                Price = 25m,
                Size = 1
            }
        },
        Status = "OK",
        RequestId = "test-request-id",
        NextUrl = nextUrl
    };

    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetTradesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(nextUrl, result.NextUrl);
}

/// <summary>
/// Tests that GetTradesAsync MarketTimestamp property converts SIP timestamp correctly.
/// </summary>
[Fact]
public async Task GetTradesAsync_MarketTimestamp_ConvertsToEasternTime()
{
    // Arrange
    var optionsTicker = "O:TSLA210903C00700000";
    var sipTimestamp = 1626965921018000000;
    var expectedResponse = new PolygonResponse<List<OptionTradeV3>>
    {
        Results = new List<OptionTradeV3>
        {
            new OptionTradeV3
            {
                Price = 25m,
                Size = 1,
                SipTimestamp = sipTimestamp
            }
        },
        Status = "OK"
    };

    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetTradesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Results);
    Assert.Single(result.Results);
    Assert.Equal(sipTimestamp, result.Results[0].SipTimestamp);
    Assert.NotNull(result.Results[0].MarketTimestamp);
    Assert.Equal("America/New_York", result.Results[0].MarketTimestamp.Value.Zone.Id);
}

/// <summary>
/// Tests that GetTradesAsync MarketParticipantTimestamp property converts participant timestamp correctly.
/// </summary>
[Fact]
public async Task GetTradesAsync_MarketParticipantTimestamp_ConvertsToEasternTime()
{
    // Arrange
    var optionsTicker = "O:TSLA210903C00700000";
    var participantTimestamp = 1626965921018000000;
    var expectedResponse = new PolygonResponse<List<OptionTradeV3>>
    {
        Results = new List<OptionTradeV3>
        {
            new OptionTradeV3
            {
                Price = 25m,
                Size = 1,
                ParticipantTimestamp = participantTimestamp
            }
        },
        Status = "OK"
    };

    _mockApi.Setup(x => x.GetTradesAsync(
        optionsTicker,
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<int?>(),
        It.IsAny<string?>(),
        It.IsAny<string?>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _service.GetTradesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Results);
    Assert.Single(result.Results);
    Assert.Equal(participantTimestamp, result.Results[0].ParticipantTimestamp);
    Assert.NotNull(result.Results[0].MarketParticipantTimestamp);
    Assert.Equal("America/New_York", result.Results[0].MarketParticipantTimestamp.Value.Zone.Id);
}


}
