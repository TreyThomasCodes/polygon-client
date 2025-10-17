// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services.Options;

/// <summary>
/// Unit tests for the OptionsService.GetSnapshotAsync method.
/// Tests the service's ability to retrieve option snapshot data from Polygon.io API.
/// </summary>
public class OptionsService_GetSnapshotTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetSnapshotTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public OptionsService_GetSnapshotTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _service = new OptionsService(_mockApi.Object);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync calls the API and returns the snapshot response.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_CallsApi_ReturnsSnapshotResponse()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionContract = "SPY251219C00650000";
        var expectedResponse = new PolygonResponse<OptionSnapshot>
        {
            Results = new OptionSnapshot
            {
                BreakEvenPrice = 685.575m,
                Day = new OptionDayData
                {
                    Change = -1.45m,
                    ChangePercent = -3.93m,
                    Close = 35.48m,
                    High = 37.71m,
                    LastUpdated = 1759982400000000000,
                    Low = 34.57m,
                    Open = 37.69m,
                    PreviousClose = 36.93m,
                    Volume = 114,
                    Vwap = 35.486m
                },
                Details = new OptionContractDetails
                {
                    ContractType = "call",
                    ExerciseStyle = "american",
                    ExpirationDate = "2025-12-19",
                    SharesPerContract = 100,
                    StrikePrice = 650,
                    Ticker = "O:SPY251219C00650000"
                },
                Greeks = new OptionGreeks
                {
                    Delta = 0.6979274194856908m,
                    Gamma = 0.006664229262249692m,
                    Theta = -0.16355871316370002m,
                    Vega = 0.9726779045118183m
                },
                ImpliedVolatility = 0.17894993694780262m,
                LastQuote = new OptionLastQuote
                {
                    Ask = 35.87m,
                    AskSize = 2,
                    AskExchange = 322,
                    Bid = 35.28m,
                    BidSize = 2,
                    BidExchange = 322,
                    LastUpdated = 1760040895024286500,
                    Midpoint = 35.575m,
                    Timeframe = "REAL-TIME"
                },
                LastTrade = new OptionLastTrade
                {
                    SipTimestamp = 1760039970128791600,
                    Conditions = new List<int> { 209 },
                    Price = 35.48m,
                    Size = 1,
                    Exchange = 312,
                    Timeframe = "REAL-TIME"
                },
                OpenInterest = 21351,
                UnderlyingAsset = new OptionUnderlyingAsset
                {
                    ChangeToBreakEven = 13.955m,
                    LastUpdated = 1760053419417818600,
                    Price = 671.62m,
                    Ticker = "SPY",
                    Timeframe = "REAL-TIME"
                }
            },
            Status = "OK",
            RequestId = "b6f7a44e2c7e897e87a39a18396e3549"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.BreakEvenPrice, result.Results.BreakEvenPrice);
        Assert.NotNull(result.Results.Day);
        Assert.Equal(expectedResponse.Results.Day.Close, result.Results.Day.Close);
        Assert.NotNull(result.Results.Details);
        Assert.Equal(expectedResponse.Results.Details.ContractType, result.Results.Details.ContractType);
        Assert.NotNull(result.Results.Greeks);
        Assert.Equal(expectedResponse.Results.Greeks.Delta, result.Results.Greeks.Delta);
        Assert.Equal(expectedResponse.Results.ImpliedVolatility, result.Results.ImpliedVolatility);
        Assert.NotNull(result.Results.LastQuote);
        Assert.Equal(expectedResponse.Results.LastQuote.Ask, result.Results.LastQuote.Ask);
        Assert.NotNull(result.Results.LastTrade);
        Assert.Equal(expectedResponse.Results.LastTrade.Price, result.Results.LastTrade.Price);
        Assert.Equal(expectedResponse.Results.OpenInterest, result.Results.OpenInterest);
        Assert.NotNull(result.Results.UnderlyingAsset);
        Assert.Equal(expectedResponse.Results.UnderlyingAsset.Price, result.Results.UnderlyingAsset.Price);

        _mockApi.Verify(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_PassesCancellationToken()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionContract = "SPY251219C00650000";
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<OptionSnapshot> { Status = "OK" };

        _mockApi.Setup(x => x.GetSnapshotAsync(underlyingAsset, optionContract, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetSnapshotAsync(underlyingAsset, optionContract, cancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetSnapshotAsync(underlyingAsset, optionContract, cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles different option contracts correctly.
    /// </summary>
    [Theory]
    [InlineData("SPY", "SPY251219C00650000")]
    [InlineData("AAPL", "AAPL250117P00150000")]
    [InlineData("TSLA", "TSLA260115C01000000")]
    [InlineData("MSFT", "MSFT250321P00400000")]
    public async Task GetSnapshotAsync_WithDifferentContracts_CallsApiWithCorrectParameters(
        string underlyingAsset, string optionContract)
    {
        // Arrange
        var expectedResponse = new PolygonResponse<OptionSnapshot>
        {
            Results = new OptionSnapshot
            {
                Details = new OptionContractDetails
                {
                    Ticker = $"O:{optionContract}"
                },
                UnderlyingAsset = new OptionUnderlyingAsset
                {
                    Ticker = underlyingAsset
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(underlyingAsset, result.Results?.UnderlyingAsset?.Ticker);
        _mockApi.Verify(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var underlyingAsset = "INVALID";
        var optionContract = "INVALID000000C00000000";
        _mockApi.Setup(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<OptionSnapshot>)null!);

        // Act
        var result = await _service.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionContract = "SPY251219C00650000";
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionContract = "SPY251219C00650000";
        var expectedResponse = new PolygonResponse<OptionSnapshot>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Results);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles put option snapshot correctly.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WithPutOption_ReturnsCorrectSnapshot()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionContract = "SPY251219P00650000";
        var expectedResponse = new PolygonResponse<OptionSnapshot>
        {
            Results = new OptionSnapshot
            {
                BreakEvenPrice = 614.425m,
                Details = new OptionContractDetails
                {
                    ContractType = "put",
                    ExerciseStyle = "american",
                    Ticker = "O:SPY251219P00650000",
                    StrikePrice = 650m
                },
                Greeks = new OptionGreeks
                {
                    Delta = -0.3020725805143092m
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal("put", result.Results.Details?.ContractType);
        Assert.True(result.Results.Greeks?.Delta < 0);
    }

    /// <summary>
    /// Tests that GetSnapshotAsync handles responses with minimal data correctly.
    /// </summary>
    [Fact]
    public async Task GetSnapshotAsync_WithMinimalData_ReturnsPartialSnapshot()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var optionContract = "SPY251219C00650000";
        var expectedResponse = new PolygonResponse<OptionSnapshot>
        {
            Results = new OptionSnapshot
            {
                Details = new OptionContractDetails
                {
                    Ticker = "O:SPY251219C00650000"
                }
                // Only basic data, other fields are null
            },
            Status = "OK",
            RequestId = "minimal-data-request"
        };

        _mockApi.Setup(x => x.GetSnapshotAsync(underlyingAsset, optionContract, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSnapshotAsync(underlyingAsset, optionContract, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.NotNull(result.Results.Details);
        Assert.Equal(expectedResponse.Results.Details.Ticker, result.Results.Details.Ticker);

        // Verify that other fields can be null without issues
        Assert.Null(result.Results.BreakEvenPrice);
        Assert.Null(result.Results.Day);
        Assert.Null(result.Results.Greeks);
        Assert.Null(result.Results.ImpliedVolatility);
        Assert.Null(result.Results.LastQuote);
        Assert.Null(result.Results.LastTrade);
        Assert.Null(result.Results.OpenInterest);
        Assert.Null(result.Results.UnderlyingAsset);
    }
}
