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

    #region GetLastTradeAsync Tests

    /// <summary>
    /// Tests that GetLastTradeAsync calls the API and returns the last trade response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_CallsApi_ReturnsLastTradeResponse()
    {
        // Arrange
        var optionsTicker = "O:TSLA260320C00700000";
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = "O:TSLA260320C00700000",
                Conditions = new List<int> { 227 },
                Id = "",
                Price = 12.3m,
                Sequence = 1527662501,
                Size = 5,
                Timestamp = 1760121506853685800,
                Exchange = 301
            },
            Status = "OK",
            RequestId = "6d9f08392336a5dadbf344014cafe294"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.Conditions, result.Results.Conditions);
        Assert.Equal(expectedResponse.Results.Id, result.Results.Id);
        Assert.Equal(expectedResponse.Results.Price, result.Results.Price);
        Assert.Equal(expectedResponse.Results.Sequence, result.Results.Sequence);
        Assert.Equal(expectedResponse.Results.Size, result.Results.Size);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.Exchange, result.Results.Exchange);

        _mockApi.Verify(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_PassesCancellationToken()
    {
        // Arrange
        var optionsTicker = "O:TSLA260320C00700000";
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<OptionTrade> { Status = "OK" };

        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetLastTradeAsync(optionsTicker, cancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetLastTradeAsync(optionsTicker, cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles different options ticker symbols correctly.
    /// </summary>
    [Theory]
    [InlineData("O:SPY251219C00650000")]
    [InlineData("O:AAPL250117P00150000")]
    [InlineData("O:TSLA260115C01000000")]
    [InlineData("O:MSFT250321P00400000")]
    public async Task GetLastTradeAsync_WithDifferentTickers_CallsApiWithCorrectTicker(string optionsTicker)
    {
        // Arrange
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = optionsTicker,
                Price = 12.5m,
                Size = 10
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(optionsTicker, result.Results?.Ticker);
        _mockApi.Verify(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var optionsTicker = "O:INVALID000000C00000000";
        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<OptionTrade>)null!);

        // Act
        var result = await _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Results);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles complete trade data with all fields populated.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithCompleteTradeData_ReturnsCompleteData()
    {
        // Arrange
        var optionsTicker = "O:TSLA260320C00700000";
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = "O:TSLA260320C00700000",
                Conditions = new List<int> { 227, 228 },
                Id = "12345",
                Price = 12.3m,
                Sequence = 1527662501,
                Size = 5,
                Timestamp = 1760121506853685800,
                Exchange = 301
            },
            Status = "OK",
            RequestId = "6d9f08392336a5dadbf344014cafe294"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.Conditions, result.Results.Conditions);
        Assert.Equal(expectedResponse.Results.Id, result.Results.Id);
        Assert.Equal(expectedResponse.Results.Price, result.Results.Price);
        Assert.Equal(expectedResponse.Results.Sequence, result.Results.Sequence);
        Assert.Equal(expectedResponse.Results.Size, result.Results.Size);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.Exchange, result.Results.Exchange);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles put option trades correctly.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithPutOption_ReturnsCorrectTrade()
    {
        // Arrange
        var optionsTicker = "O:SPY251219P00650000";
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = optionsTicker,
                Price = 8.75m,
                Size = 3,
                Timestamp = 1760121506853685800,
                Exchange = 312
            },
            Status = "OK",
            RequestId = "test-put-request-id"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal(optionsTicker, result.Results.Ticker);
        Assert.Contains('P', optionsTicker);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles responses with minimal data correctly.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithMinimalData_ReturnsPartialTrade()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = "O:SPY251219C00650000",
                Price = 12.3m
                // Only basic data, other fields are null
            },
            Status = "OK",
            RequestId = "minimal-data-request"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.Price, result.Results.Price);

        // Verify that other fields can be null without issues
        Assert.Null(result.Results.Conditions);
        Assert.Null(result.Results.Id);
        Assert.Null(result.Results.Sequence);
        Assert.Null(result.Results.Size);
        Assert.Null(result.Results.Timestamp);
        Assert.Null(result.Results.Exchange);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles trade with multiple condition codes.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithMultipleConditions_ReturnsAllConditions()
    {
        // Arrange
        var optionsTicker = "O:AAPL250117C00150000";
        var conditions = new List<int> { 227, 228, 229 };
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = optionsTicker,
                Conditions = conditions,
                Price = 5.25m,
                Size = 10,
                Timestamp = 1760121506853685800,
                Exchange = 301
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal(3, result.Results.Conditions?.Count);
        Assert.Equal(conditions, result.Results.Conditions);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync MarketTimestamp property converts timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_MarketTimestamp_ConvertsToEasternTime()
    {
        // Arrange
        var optionsTicker = "O:SPY251219C00650000";
        var timestamp = 1760121506853685800;
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = optionsTicker,
                Timestamp = timestamp,
                Price = 12.3m
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(optionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(optionsTicker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal(timestamp, result.Results.Timestamp);
        Assert.NotNull(result.Results.MarketTimestamp);
        Assert.Equal("America/New_York", result.Results.MarketTimestamp.Value.Zone.Id);
    }

    #endregion

    #region GetQuotesAsync Tests

    /// <summary>
    /// Tests that GetQuotesAsync calls the API and returns the quotes response.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_CallsApi_ReturnsQuotesResponse()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>
            {
                new OptionQuote
                {
                    AskExchange = 303,
                    AskPrice = 294.98m,
                    AskSize = 1,
                    BidExchange = 303,
                    BidPrice = 284.98m,
                    BidSize = 2,
                    SequenceNumber = 202406,
                    SipTimestamp = 1646663401868127500
                },
                new OptionQuote
                {
                    AskExchange = 303,
                    AskPrice = 294.98m,
                    AskSize = 2,
                    BidExchange = 303,
                    BidPrice = 284.98m,
                    BidSize = 2,
                    SequenceNumber = 202407,
                    SipTimestamp = 1646663401868127500
                }
            },
            Status = "OK",
            RequestId = "582bd263f011ded5e3a8244ebf045fc0",
            NextUrl = "https://api.polygon.io/v3/quotes/O:SPY241220P00720000?cursor=test"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        var result = await _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Equal(expectedResponse.NextUrl, result.NextUrl);
        Assert.NotNull(result.Results);
        Assert.Equal(2, result.Results.Count);
        Assert.Equal(294.98m, result.Results[0].AskPrice);
        Assert.Equal(284.98m, result.Results[0].BidPrice);
        Assert.Equal(202406, result.Results[0].SequenceNumber);

        _mockApi.Verify(x => x.GetQuotesAsync(
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
    /// Tests that GetQuotesAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_PassesCancellationToken()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<List<OptionQuote>> { Status = "OK" };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        await _service.GetQuotesAsync(optionsTicker, cancellationToken: cancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetQuotesAsync(
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
    /// Tests that GetQuotesAsync passes all parameters correctly to the API.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithAllParameters_PassesParametersCorrectly()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var timestamp = "2022-03-07";
        var timestampLt = "2022-03-08";
        var timestampLte = "2022-03-07T23:59:59";
        var timestampGt = "2022-03-06";
        var timestampGte = "2022-03-07T00:00:00";
        var order = "asc";
        var limit = 10;
        var sort = "timestamp";
        var cursor = "test-cursor";
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>(),
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        var result = await _service.GetQuotesAsync(
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
        _mockApi.Verify(x => x.GetQuotesAsync(
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
    /// Tests that GetQuotesAsync handles different options ticker symbols correctly.
    /// </summary>
    [Theory]
    [InlineData("O:SPY241220P00720000")]
    [InlineData("O:AAPL250117C00150000")]
    [InlineData("O:TSLA260115P01000000")]
    [InlineData("O:MSFT250321C00400000")]
    public async Task GetQuotesAsync_WithDifferentTickers_CallsApiWithCorrectTicker(string optionsTicker)
    {
        // Arrange
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>
            {
                new OptionQuote
                {
                    AskPrice = 10.5m,
                    BidPrice = 10.0m
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        var result = await _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockApi.Verify(x => x.GetQuotesAsync(
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
    /// Tests that GetQuotesAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var optionsTicker = "O:INVALID000000C00000000";
        _mockApi.Setup(x => x.GetQuotesAsync(
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
            .ReturnsAsync((PolygonResponse<List<OptionQuote>>)null!);

        // Act
        var result = await _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetQuotesAsync(
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
    /// Tests that GetQuotesAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetQuotesAsync(
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
            () => _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetQuotesAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        var result = await _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Results);
    }

    /// <summary>
    /// Tests that GetQuotesAsync handles empty results list.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithEmptyResults_ReturnsEmptyList()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>(),
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        var result = await _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Empty(result.Results);
    }

    /// <summary>
    /// Tests that GetQuotesAsync handles pagination with next_url correctly.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithPagination_ReturnsNextUrl()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var nextUrl = "https://api.polygon.io/v3/quotes/O:SPY241220P00720000?cursor=test-cursor";
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>
            {
                new OptionQuote
                {
                    AskPrice = 294.98m,
                    BidPrice = 284.98m
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            NextUrl = nextUrl
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        var result = await _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nextUrl, result.NextUrl);
    }

    /// <summary>
    /// Tests that GetQuotesAsync with timestamp filter passes the parameter correctly.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithTimestampFilter_PassesTimestampCorrectly()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var timestamp = "2022-03-07T14:30:00";
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>
            {
                new OptionQuote
                {
                    AskPrice = 294.98m,
                    BidPrice = 284.98m,
                    SipTimestamp = 1646663401868127500
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
            optionsTicker,
            timestamp,
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
        var result = await _service.GetQuotesAsync(optionsTicker, timestamp: timestamp, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockApi.Verify(x => x.GetQuotesAsync(
            optionsTicker,
            timestamp,
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
    /// Tests that GetQuotesAsync MarketTimestamp property converts timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_MarketTimestamp_ConvertsToEasternTime()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var sipTimestamp = 1646663401868127500;
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>
            {
                new OptionQuote
                {
                    AskPrice = 294.98m,
                    BidPrice = 284.98m,
                    SipTimestamp = sipTimestamp
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        var result = await _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Single(result.Results);
        Assert.Equal(sipTimestamp, result.Results[0].SipTimestamp);
        Assert.NotNull(result.Results[0].MarketTimestamp);
        Assert.Equal("America/New_York", result.Results[0].MarketTimestamp.Value.Zone.Id);
    }

    /// <summary>
    /// Tests that GetQuotesAsync handles complete quote data with all fields populated.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithCompleteQuoteData_ReturnsCompleteData()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>
            {
                new OptionQuote
                {
                    AskExchange = 303,
                    AskPrice = 294.98m,
                    AskSize = 1,
                    BidExchange = 303,
                    BidPrice = 284.98m,
                    BidSize = 2,
                    SequenceNumber = 202406,
                    SipTimestamp = 1646663401868127500
                }
            },
            Status = "OK",
            RequestId = "582bd263f011ded5e3a8244ebf045fc0"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        var result = await _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Single(result.Results);
        var quote = result.Results[0];
        Assert.Equal(expectedResponse.Results[0].AskExchange, quote.AskExchange);
        Assert.Equal(expectedResponse.Results[0].AskPrice, quote.AskPrice);
        Assert.Equal(expectedResponse.Results[0].AskSize, quote.AskSize);
        Assert.Equal(expectedResponse.Results[0].BidExchange, quote.BidExchange);
        Assert.Equal(expectedResponse.Results[0].BidPrice, quote.BidPrice);
        Assert.Equal(expectedResponse.Results[0].BidSize, quote.BidSize);
        Assert.Equal(expectedResponse.Results[0].SequenceNumber, quote.SequenceNumber);
        Assert.Equal(expectedResponse.Results[0].SipTimestamp, quote.SipTimestamp);
    }

    /// <summary>
    /// Tests that GetQuotesAsync handles responses with minimal data correctly.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithMinimalData_ReturnsPartialQuote()
    {
        // Arrange
        var optionsTicker = "O:SPY241220P00720000";
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>
            {
                new OptionQuote
                {
                    AskPrice = 294.98m,
                    BidPrice = 284.98m
                    // Only basic data, other fields are null
                }
            },
            Status = "OK",
            RequestId = "minimal-data-request"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
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
        var result = await _service.GetQuotesAsync(optionsTicker, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Single(result.Results);
        var quote = result.Results[0];
        Assert.Equal(expectedResponse.Results[0].AskPrice, quote.AskPrice);
        Assert.Equal(expectedResponse.Results[0].BidPrice, quote.BidPrice);

        // Verify that other fields can be null without issues
        Assert.Null(quote.AskExchange);
        Assert.Null(quote.AskSize);
        Assert.Null(quote.BidExchange);
        Assert.Null(quote.BidSize);
        Assert.Null(quote.SequenceNumber);
        Assert.Null(quote.SipTimestamp);
    }

    #endregion

    #region GetTradesAsync Tests

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

    #endregion

    #region GetBarsAsync Tests

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

    #endregion

    #region GetDailyOpenCloseAsync Tests

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

    #endregion

    #region GetPreviousDayBarAsync Tests

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

    #endregion
}
