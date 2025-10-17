// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services.Options;

/// <summary>
/// Unit tests for the OptionsService.GetContractDetailsAsync method.
/// Tests the service's ability to retrieve options contract details from Polygon.io API.
/// </summary>
public class OptionsService_GetContractDetailsTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetContractDetailsTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public OptionsService_GetContractDetailsTests()
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
}
