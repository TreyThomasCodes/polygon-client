// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the OptionsService class.
/// Tests the service's ability to retrieve options contract data from Polygon.io API.
/// </summary>
public class OptionsServiceTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsServiceTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public OptionsServiceTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _service = new OptionsService(_mockApi.Object);
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when api parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenApiIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OptionsService(null!));
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync calls the API and returns the contract details response.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_CallsApi_ReturnsContractDetailsResponse()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var expectedResponse = new PolygonResponse<OptionsContract>
        {
            Results = new OptionsContract
            {
                Cfi = "OCASPS",
                ContractType = "call",
                ExerciseStyle = "american",
                ExpirationDate = "2025-12-19",
                PrimaryExchange = "BATO",
                SharesPerContract = 100,
                StrikePrice = 650,
                Ticker = "O:SPY251219C00650000",
                UnderlyingTicker = "SPY"
            },
            Status = "OK",
            RequestId = "d0d9e7c5e58988747dabf332bdb629f7"
        };

        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Cfi, result.Results.Cfi);
        Assert.Equal(expectedResponse.Results.ContractType, result.Results.ContractType);
        Assert.Equal(expectedResponse.Results.ExerciseStyle, result.Results.ExerciseStyle);
        Assert.Equal(expectedResponse.Results.ExpirationDate, result.Results.ExpirationDate);
        Assert.Equal(expectedResponse.Results.PrimaryExchange, result.Results.PrimaryExchange);
        Assert.Equal(expectedResponse.Results.SharesPerContract, result.Results.SharesPerContract);
        Assert.Equal(expectedResponse.Results.StrikePrice, result.Results.StrikePrice);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.UnderlyingTicker, result.Results.UnderlyingTicker);

        _mockApi.Verify(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_PassesCancellationToken()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<OptionsContract> { Status = "OK" };

        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetContractDetailsAsync(optionsTicker, cancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetContractDetailsAsync(optionsTicker, cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles different options ticker symbols correctly.
    /// </summary>
    [Theory]
    [InlineData("O:SPY251219C00650000")]
    [InlineData("O:AAPL250117P00150000")]
    [InlineData("O:TSLA260115C01000000")]
    [InlineData("O:MSFT250321P00400000")]
    public async Task GetContractDetailsAsync_WithDifferentTickers_CallsApiWithCorrectTicker(string optionsTicker)
    {
        // Arrange
        var expectedResponse = new PolygonResponse<OptionsContract>
        {
            Results = new OptionsContract
            {
                Ticker = optionsTicker,
                ContractType = optionsTicker.Contains('C') ? "call" : "put"
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(optionsTicker, result.Results?.Ticker);
        _mockApi.Verify(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var optionsTicker = "O:INVALID000000C00000000";
        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<OptionsContract>)null!);

        // Act
        var result = await _service.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var expectedResponse = new PolygonResponse<OptionsContract>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Results);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles complete contract data with all fields populated.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithCompleteContractData_ReturnsCompleteData()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var expectedResponse = new PolygonResponse<OptionsContract>
        {
            Results = new OptionsContract
            {
                Cfi = "OCASPS",
                ContractType = "call",
                ExerciseStyle = "american",
                ExpirationDate = "2025-12-19",
                PrimaryExchange = "BATO",
                SharesPerContract = 100,
                StrikePrice = 650.00m,
                Ticker = "O:SPY251219C00650000",
                UnderlyingTicker = "SPY"
            },
            Status = "OK",
            RequestId = "d0d9e7c5e58988747dabf332bdb629f7"
        };

        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Cfi, result.Results.Cfi);
        Assert.Equal(expectedResponse.Results.ContractType, result.Results.ContractType);
        Assert.Equal(expectedResponse.Results.ExerciseStyle, result.Results.ExerciseStyle);
        Assert.Equal(expectedResponse.Results.ExpirationDate, result.Results.ExpirationDate);
        Assert.Equal(expectedResponse.Results.PrimaryExchange, result.Results.PrimaryExchange);
        Assert.Equal(expectedResponse.Results.SharesPerContract, result.Results.SharesPerContract);
        Assert.Equal(expectedResponse.Results.StrikePrice, result.Results.StrikePrice);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.UnderlyingTicker, result.Results.UnderlyingTicker);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles put options correctly.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithPutOption_ReturnsCorrectContractType()
    {
        // Arrange
        var optionsTicker = "O:SPY251219P00650000";
        var expectedResponse = new PolygonResponse<OptionsContract>
        {
            Results = new OptionsContract
            {
                Cfi = "OPASPS",
                ContractType = "put",
                ExerciseStyle = "american",
                ExpirationDate = "2025-12-19",
                PrimaryExchange = "BATO",
                SharesPerContract = 100,
                StrikePrice = 650.00m,
                Ticker = "O:SPY251219P00650000",
                UnderlyingTicker = "SPY"
            },
            Status = "OK",
            RequestId = "test-put-request-id"
        };

        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal("put", result.Results.ContractType);
        Assert.Equal(optionsTicker, result.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles European style options correctly.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithEuropeanStyle_ReturnsCorrectExerciseStyle()
    {
        // Arrange
        var optionsTicker = "O:SPX251219C04650000";
        var expectedResponse = new PolygonResponse<OptionsContract>
        {
            Results = new OptionsContract
            {
                Cfi = "OCESPS",
                ContractType = "call",
                ExerciseStyle = "european",
                ExpirationDate = "2025-12-19",
                PrimaryExchange = "CBOE",
                SharesPerContract = 100,
                StrikePrice = 4650.00m,
                Ticker = "O:SPX251219C04650000",
                UnderlyingTicker = "SPX"
            },
            Status = "OK",
            RequestId = "test-european-request-id"
        };

        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal("european", result.Results.ExerciseStyle);
        Assert.Equal(optionsTicker, result.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles responses with minimal data correctly.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithMinimalData_ReturnsPartialContract()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var expectedResponse = new PolygonResponse<OptionsContract>
        {
            Results = new OptionsContract
            {
                Ticker = "O:SPY251219C00650000",
                UnderlyingTicker = "SPY"
                // Only basic data, other fields are null
            },
            Status = "OK",
            RequestId = "minimal-data-request"
        };

        _mockApi.Setup(x => x.GetContractDetailsAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.UnderlyingTicker, result.Results.UnderlyingTicker);

        // Verify that other fields can be null without issues
        Assert.Null(result.Results.Cfi);
        Assert.Null(result.Results.ContractType);
        Assert.Null(result.Results.ExerciseStyle);
        Assert.Null(result.Results.ExpirationDate);
        Assert.Null(result.Results.PrimaryExchange);
        Assert.Null(result.Results.SharesPerContract);
        Assert.Null(result.Results.StrikePrice);
    }

    #region GetSnapshotAsync Tests

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

    #endregion

    #region GetChainSnapshotAsync Tests

    /// <summary>
    /// Tests that GetChainSnapshotAsync calls the API and returns the chain snapshot response.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_CallsApi_ReturnsChainSnapshotResponse()
    {
        // Arrange
        var underlyingAsset = "MSTR";
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    BreakEvenPrice = 310.85m,
                    Day = new OptionDayData
                    {
                        Change = 0m,
                        ChangePercent = 0m,
                        Close = 172.65m,
                        High = 172.65m,
                        LastUpdated = 1760040000000000000,
                        Low = 172.65m,
                        Open = 172.65m,
                        PreviousClose = 172.65m,
                        Volume = 5,
                        Vwap = 172.65m
                    },
                    Details = new OptionContractDetails
                    {
                        ContractType = "call",
                        ExerciseStyle = "american",
                        ExpirationDate = "2025-10-10",
                        SharesPerContract = 100,
                        StrikePrice = 150,
                        Ticker = "O:MSTR251010C00150000"
                    },
                    Greeks = new OptionGreeks
                    {
                        Delta = 0.9951282876879873m,
                        Gamma = 0.00014861413681600426m,
                        Theta = -2.053186766077909m,
                        Vega = 0.0013923356112067263m
                    },
                    ImpliedVolatility = 10.17106034370156m,
                    LastQuote = new OptionLastQuote
                    {
                        Ask = 161.85m,
                        AskSize = 13,
                        AskExchange = 301,
                        Bid = 159.85m,
                        BidSize = 13,
                        BidExchange = 301,
                        LastUpdated = 1760112568871916300,
                        Midpoint = 160.85m,
                        Timeframe = "REAL-TIME"
                    },
                    LastTrade = new OptionLastTrade
                    {
                        SipTimestamp = 1760020832057391000,
                        Conditions = new List<int> { 231 },
                        Price = 172.65m,
                        Size = 5,
                        Exchange = 301,
                        Timeframe = "REAL-TIME"
                    },
                    OpenInterest = 47,
                    UnderlyingAsset = new OptionUnderlyingAsset
                    {
                        ChangeToBreakEven = 0.17m,
                        LastUpdated = 1760112614961306000,
                        Price = 310.68m,
                        Ticker = "MSTR",
                        Timeframe = "REAL-TIME"
                    }
                },
                new OptionSnapshot
                {
                    BreakEvenPrice = 311.125m,
                    Details = new OptionContractDetails
                    {
                        ContractType = "call",
                        ExerciseStyle = "american",
                        ExpirationDate = "2025-10-10",
                        SharesPerContract = 100,
                        StrikePrice = 160,
                        Ticker = "O:MSTR251010C00160000"
                    },
                    OpenInterest = 9
                }
            },
            Status = "OK",
            RequestId = "da2e6e0f4063e80346179d18ca3a5fd8",
            NextUrl = "https://api.polygon.io/v3/snapshot/options/MSTR?cursor=YXA9TyUzQU1TVFIyNTEwMTBDMDAyMTI1MDAlM0EyMTIuNTAmYXM9JmV4cGlyYXRpb25fZGF0ZS5ndGU9MjAyNS0xMC0xMCZsaW1pdD0xMCZvcmRlcj1hc2Mmc29ydD10aWNrZXI"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(underlyingAsset, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Equal(expectedResponse.NextUrl, result.NextUrl);
        Assert.NotNull(result.Results);
        Assert.Equal(2, result.Results.Count);
        Assert.Equal(310.85m, result.Results[0].BreakEvenPrice);
        Assert.Equal("O:MSTR251010C00150000", result.Results[0].Details?.Ticker);
        Assert.Equal("O:MSTR251010C00160000", result.Results[1].Details?.Ticker);

        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync passes all parameters correctly to the API.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithAllParameters_PassesParametersCorrectly()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var strikePrice = 650m;
        var contractType = "call";
        var expirationDateGte = "2025-01-01";
        var expirationDateLte = "2025-12-31";
        var limit = 10;
        var order = "asc";
        var sort = "strike_price";
        var cursor = "test-cursor";
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>(),
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            strikePrice,
            contractType,
            expirationDateGte,
            expirationDateLte,
            limit,
            order,
            sort,
            cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(
            underlyingAsset,
            strikePrice,
            contractType,
            expirationDateGte,
            expirationDateLte,
            limit,
            order,
            sort,
            cursor,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            strikePrice,
            contractType,
            expirationDateGte,
            expirationDateLte,
            limit,
            order,
            sort,
            cursor,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_PassesCancellationToken()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>> { Status = "OK" };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetChainSnapshotAsync(underlyingAsset, cancellationToken: cancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles different underlying assets correctly.
    /// </summary>
    [Theory]
    [InlineData("SPY")]
    [InlineData("AAPL")]
    [InlineData("TSLA")]
    [InlineData("MSTR")]
    public async Task GetChainSnapshotAsync_WithDifferentUnderlyingAssets_CallsApiWithCorrectAsset(string underlyingAsset)
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    UnderlyingAsset = new OptionUnderlyingAsset
                    {
                        Ticker = underlyingAsset
                    }
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(underlyingAsset, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(underlyingAsset, result.Results?[0].UnderlyingAsset?.Ticker);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var underlyingAsset = "INVALID";
        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<List<OptionSnapshot>>)null!);

        // Act
        var result = await _service.GetChainSnapshotAsync(underlyingAsset, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetChainSnapshotAsync(underlyingAsset, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(underlyingAsset, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Results);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles empty results list.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithEmptyResults_ReturnsEmptyList()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>(),
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(underlyingAsset, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Empty(result.Results);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync with strike price filter passes the parameter correctly.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithStrikePrice_PassesStrikePriceCorrectly()
    {
        // Arrange
        var underlyingAsset = "SPY";
        var strikePrice = 650m;
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    Details = new OptionContractDetails
                    {
                        StrikePrice = strikePrice
                    }
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            strikePrice,
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(underlyingAsset, strikePrice: strikePrice, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(strikePrice, result.Results?[0].Details?.StrikePrice);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            strikePrice,
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync with contract type filter returns only the specified type.
    /// </summary>
    [Theory]
    [InlineData("call")]
    [InlineData("put")]
    public async Task GetChainSnapshotAsync_WithContractType_FiltersCorrectly(string contractType)
    {
        // Arrange
        var underlyingAsset = "SPY";
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    Details = new OptionContractDetails
                    {
                        ContractType = contractType
                    }
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            contractType,
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(underlyingAsset, contractType: contractType, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contractType, result.Results?[0].Details?.ContractType);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            contractType,
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles pagination with next_url correctly.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithPagination_ReturnsNextUrl()
    {
        // Arrange
        var underlyingAsset = "MSTR";
        var nextUrl = "https://api.polygon.io/v3/snapshot/options/MSTR?cursor=test-cursor";
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    Details = new OptionContractDetails
                    {
                        Ticker = "O:MSTR251010C00150000"
                    }
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            NextUrl = nextUrl
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(underlyingAsset, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nextUrl, result.NextUrl);
    }

    #endregion
}
