// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Exceptions;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services.Options;

/// <summary>
/// Unit tests for the OptionsService.GetContractDetailsAsync method.
/// Tests the service's ability to retrieve options contract details from Polygon.io API.
/// </summary>
public class OptionsService_GetContractDetailsTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetContractDetailsRequest>> _mockValidator;
    private readonly Mock<ILogger<OptionsService>> _mockLogger;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetContractDetailsTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public OptionsService_GetContractDetailsTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetContractDetailsRequest>>();
        _mockLogger = new Mock<ILogger<OptionsService>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetContractDetailsRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetContractDetailsRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new OptionsService(_mockApi.Object, _mockServiceProvider.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when api parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenApiIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OptionsService(null!, _mockServiceProvider.Object, _mockLogger.Object));
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when serviceProvider parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenServiceProviderIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OptionsService(_mockApi.Object, null!, _mockLogger.Object));
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when logger parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OptionsService(_mockApi.Object, _mockServiceProvider.Object, null!));
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync validates the request and calls the API.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_ValidatesRequest_AndCallsApi()
    {
        // Arrange
        var request = new GetContractDetailsRequest { OptionsTicker = "O:SPY251219C00650000" };
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

        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetContractDetailsRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync calls the API and returns the contract details response.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_CallsApi_ReturnsContractDetailsResponse()
    {
        // Arrange
        var request = new GetContractDetailsRequest { OptionsTicker = "O:SPY251219C00650000" };
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

        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken);

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

        _mockApi.Verify(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new GetContractDetailsRequest { OptionsTicker = "O:SPY251219C00650000" };
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<OptionsContract> { Status = "OK" };

        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetContractDetailsAsync(request, cancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetContractDetailsRequest>>(ctx => ctx.InstanceToValidate == request), cancellationToken), Times.Once);
        _mockApi.Verify(x => x.GetContractDetailsAsync(request.OptionsTicker, cancellationToken), Times.Once);
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
        var request = new GetContractDetailsRequest { OptionsTicker = optionsTicker };
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
        var result = await _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetContractDetailsRequest { OptionsTicker = "O:INVALID000000C00000000" };
        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<OptionsContract>)null!);

        // Act
        var result = await _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that the service wraps API exceptions in PolygonHttpException.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var request = new GetContractDetailsRequest { OptionsTicker = "O:SPY251219C00650000" };
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<PolygonHttpException>(
            () => _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken));
        Assert.NotNull(actualException.InnerException);
        Assert.Equal(expectedException.Message, actualException.InnerException.Message);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync throws PolygonValidationException when request is invalid.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new GetContractDetailsRequest { OptionsTicker = "" };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("OptionsTicker", "Options ticker must not be empty.")
        };
        var validationResult = new ValidationResult(validationFailures);

        // Reset and setup the validator to throw PolygonValidationException directly
        _mockValidator.Reset();
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetContractDetailsRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<PolygonValidationException>(
            () => _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken));

        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetContractDetailsRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetContractDetailsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var request = new GetContractDetailsRequest { OptionsTicker = "O:SPY251219C00650000" };
        var expectedResponse = new PolygonResponse<OptionsContract>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetContractDetailsRequest { OptionsTicker = "O:SPY251219C00650000" };
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

        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetContractDetailsRequest { OptionsTicker = "O:SPY251219P00650000" };
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

        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal("put", result.Results.ContractType);
        Assert.Equal(request.OptionsTicker, result.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles European style options correctly.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithEuropeanStyle_ReturnsCorrectExerciseStyle()
    {
        // Arrange
        var request = new GetContractDetailsRequest { OptionsTicker = "O:SPX251219C04650000" };
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

        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal("european", result.Results.ExerciseStyle);
        Assert.Equal(request.OptionsTicker, result.Results.Ticker);
    }

    /// <summary>
    /// Tests that GetContractDetailsAsync handles responses with minimal data correctly.
    /// </summary>
    [Fact]
    public async Task GetContractDetailsAsync_WithMinimalData_ReturnsPartialContract()
    {
        // Arrange
        var request = new GetContractDetailsRequest { OptionsTicker = "O:SPY251219C00650000" };
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

        _mockApi.Setup(x => x.GetContractDetailsAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetContractDetailsAsync(request, TestContext.Current.CancellationToken);

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
