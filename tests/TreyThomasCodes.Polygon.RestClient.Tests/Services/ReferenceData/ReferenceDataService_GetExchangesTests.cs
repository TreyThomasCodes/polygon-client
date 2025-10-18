// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Exceptions;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.Models.Common;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the ReferenceDataService.GetExchangesAsync method.
/// Tests the service's ability to retrieve exchanges from Polygon.io API.
/// </summary>
public class ReferenceDataService_GetExchangesTests
{
    private readonly Mock<IPolygonReferenceApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetExchangesRequest>> _mockValidator;
    private readonly Mock<ILogger<ReferenceDataService>> _mockLogger;
    private readonly ReferenceDataService _service;

    /// <summary>
    /// Initializes a new instance of the ReferenceDataService_GetExchangesTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public ReferenceDataService_GetExchangesTests()
    {
        _mockApi = new Mock<IPolygonReferenceApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetExchangesRequest>>();
        _mockLogger = new Mock<ILogger<ReferenceDataService>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetExchangesRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetExchangesRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new ReferenceDataService(_mockApi.Object, _mockServiceProvider.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when api parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenApiIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ReferenceDataService(null!, _mockServiceProvider.Object, _mockLogger.Object));
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when serviceProvider parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenServiceProviderIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ReferenceDataService(_mockApi.Object, null!, _mockLogger.Object));
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when logger parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ReferenceDataService(_mockApi.Object, _mockServiceProvider.Object, null!));
    }

    /// <summary>
    /// Tests that GetExchangesAsync validates the request and calls the API.
    /// </summary>
    [Fact]
    public async Task GetExchangesAsync_ValidatesRequest_AndCallsApi()
    {
        // Arrange
        var request = new GetExchangesRequest
        {
            AssetClass = AssetClass.Stocks,
            Locale = Locale.UnitedStates
        };
        var expectedResponse = new PolygonResponse<List<Exchange>>
        {
            Results = new List<Exchange>
            {
                new Exchange
                {
                    Id = 1,
                    Type = "exchange",
                    AssetClass = "stocks",
                    Locale = "us",
                    Name = "NYSE American, LLC",
                    Acronym = "AMEX",
                    Mic = "XASE",
                    OperatingMic = "XNYS",
                    ParticipantId = "A",
                    Url = "https://www.nyse.com/markets/nyse-american"
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            Count = 1
        };

        _mockApi.Setup(x => x.GetExchangesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<Locale?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetExchangesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<GetExchangesRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetExchangesAsync(
            request.AssetClass,
            request.Locale,
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    /// <summary>
    /// Tests that GetExchangesAsync calls the API and returns exchanges.
    /// </summary>
    [Fact]
    public async Task GetExchangesAsync_CallsApi_ReturnsExchanges()
    {
        // Arrange
        var request = new GetExchangesRequest
        {
            AssetClass = AssetClass.Stocks,
            Locale = Locale.UnitedStates
        };
        var expectedResponse = new PolygonResponse<List<Exchange>>
        {
            Results = new List<Exchange>
            {
                new Exchange
                {
                    Id = 1,
                    Type = "exchange",
                    AssetClass = "stocks",
                    Locale = "us",
                    Name = "NYSE American, LLC",
                    Acronym = "AMEX",
                    Mic = "XASE",
                    OperatingMic = "XNYS",
                    ParticipantId = "A",
                    Url = "https://www.nyse.com/markets/nyse-american"
                },
                new Exchange
                {
                    Id = 10,
                    Type = "exchange",
                    AssetClass = "stocks",
                    Locale = "us",
                    Name = "New York Stock Exchange",
                    Mic = "XNYS",
                    OperatingMic = "XNYS",
                    ParticipantId = "N",
                    Url = "https://www.nyse.com"
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            Count = 2
        };

        _mockApi.Setup(x => x.GetExchangesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<Locale?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetExchangesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);
        Assert.Equal("test-request-id", result.RequestId);
        Assert.Equal(2, result.Count);
        Assert.NotNull(result.Results);
        Assert.Equal(2, result.Results.Count);

        var nyseAmerican = result.Results[0];
        Assert.Equal(1, nyseAmerican.Id);
        Assert.Equal("exchange", nyseAmerican.Type);
        Assert.Equal("stocks", nyseAmerican.AssetClass);
        Assert.Equal("us", nyseAmerican.Locale);
        Assert.Equal("NYSE American, LLC", nyseAmerican.Name);
        Assert.Equal("AMEX", nyseAmerican.Acronym);
        Assert.Equal("XASE", nyseAmerican.Mic);
        Assert.Equal("XNYS", nyseAmerican.OperatingMic);
        Assert.Equal("A", nyseAmerican.ParticipantId);
        Assert.Equal("https://www.nyse.com/markets/nyse-american", nyseAmerican.Url);

        var nyse = result.Results[1];
        Assert.Equal(10, nyse.Id);
        Assert.Equal("exchange", nyse.Type);
        Assert.Equal("New York Stock Exchange", nyse.Name);
        Assert.Equal("XNYS", nyse.Mic);

        _mockApi.Verify(x => x.GetExchangesAsync(
            AssetClass.Stocks,
            Locale.UnitedStates,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetExchangesAsync passes all parameters correctly to the API.
    /// </summary>
    [Fact]
    public async Task GetExchangesAsync_WithAllParameters_PassesParametersToApi()
    {
        // Arrange
        var request = new GetExchangesRequest
        {
            AssetClass = AssetClass.Options,
            Locale = Locale.Global
        };
        var expectedResponse = new PolygonResponse<List<Exchange>>
        {
            Results = new List<Exchange>(),
            Status = "OK",
            RequestId = "test-request-id",
            Count = 0
        };

        _mockApi.Setup(x => x.GetExchangesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<Locale?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetExchangesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetExchangesAsync(
            AssetClass.Options,
            Locale.Global,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetExchangesAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetExchangesAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new GetExchangesRequest();
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<List<Exchange>>
        {
            Results = new List<Exchange>(),
            Status = "OK",
            RequestId = "test-request-id",
            Count = 0
        };

        _mockApi.Setup(x => x.GetExchangesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<Locale?>(),
                cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetExchangesAsync(request, cancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<GetExchangesRequest>>(), cancellationToken), Times.Once);
        _mockApi.Verify(x => x.GetExchangesAsync(
            null,
            null,
            cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetExchangesAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetExchangesAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var request = new GetExchangesRequest();
        _mockApi.Setup(x => x.GetExchangesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<Locale?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<List<Exchange>>)null!);

        // Act
        var result = await _service.GetExchangesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetExchangesAsync(
            It.IsAny<AssetClass?>(),
            It.IsAny<Locale?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that the service wraps API exceptions in PolygonHttpException.
    /// </summary>
    [Fact]
    public async Task GetExchangesAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var request = new GetExchangesRequest();
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetExchangesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<Locale?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<PolygonHttpException>(
            () => _service.GetExchangesAsync(request, TestContext.Current.CancellationToken));
        Assert.NotNull(actualException.InnerException);
        Assert.Equal(expectedException.Message, actualException.InnerException.Message);
    }

    /// <summary>
    /// Tests that GetExchangesAsync handles exchanges with different types correctly.
    /// </summary>
    [Theory]
    [InlineData(AssetClass.Stocks)]
    [InlineData(AssetClass.Options)]
    [InlineData(AssetClass.Crypto)]
    [InlineData(AssetClass.Forex)]
    public async Task GetExchangesAsync_WithDifferentAssetClasses_ReturnsCorrectData(AssetClass assetClass)
    {
        // Arrange
        var request = new GetExchangesRequest
        {
            AssetClass = assetClass
        };
        var expectedResponse = new PolygonResponse<List<Exchange>>
        {
            Results = new List<Exchange>
            {
                new Exchange
                {
                    Id = 1,
                    Type = "exchange",
                    AssetClass = assetClass.ToString().ToLower(),
                    Locale = "us",
                    Name = "Test Exchange",
                    OperatingMic = "TEST"
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            Count = 1
        };

        _mockApi.Setup(x => x.GetExchangesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<Locale?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetExchangesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Results);
        Assert.Equal(assetClass.ToString().ToLower(), result.Results[0].AssetClass);

        _mockApi.Verify(x => x.GetExchangesAsync(
            assetClass,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetExchangesAsync handles exchanges with different exchange types.
    /// </summary>
    [Theory]
    [InlineData("exchange")]
    [InlineData("TRF")]
    [InlineData("SIP")]
    [InlineData("ORF")]
    public async Task GetExchangesAsync_WithDifferentExchangeTypes_ReturnsCorrectData(string exchangeType)
    {
        // Arrange
        var request = new GetExchangesRequest();
        var expectedResponse = new PolygonResponse<List<Exchange>>
        {
            Results = new List<Exchange>
            {
                new Exchange
                {
                    Id = 1,
                    Type = exchangeType,
                    AssetClass = "stocks",
                    Locale = "us",
                    Name = "Test Exchange",
                    OperatingMic = "TEST"
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            Count = 1
        };

        _mockApi.Setup(x => x.GetExchangesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<Locale?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetExchangesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Results);
        Assert.Equal(exchangeType, result.Results[0].Type);
    }

    /// <summary>
    /// Tests that GetExchangesAsync handles exchanges with optional fields missing.
    /// </summary>
    [Fact]
    public async Task GetExchangesAsync_WithMissingOptionalFields_ReturnsCorrectData()
    {
        // Arrange
        var request = new GetExchangesRequest();
        var expectedResponse = new PolygonResponse<List<Exchange>>
        {
            Results = new List<Exchange>
            {
                new Exchange
                {
                    Id = 5,
                    Type = "SIP",
                    AssetClass = "stocks",
                    Locale = "us",
                    Name = "Unlisted Trading Privileges",
                    OperatingMic = "XNAS",
                    ParticipantId = "E",
                    Url = "https://www.utpplan.com"
                    // Acronym and Mic are null
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            Count = 1
        };

        _mockApi.Setup(x => x.GetExchangesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<Locale?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetExchangesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Results);

        var exchange = result.Results[0];
        Assert.Equal(5, exchange.Id);
        Assert.Equal("SIP", exchange.Type);
        Assert.Equal("Unlisted Trading Privileges", exchange.Name);
        Assert.Null(exchange.Acronym);
        Assert.Null(exchange.Mic);
        Assert.Equal("XNAS", exchange.OperatingMic);
        Assert.Equal("E", exchange.ParticipantId);
        Assert.Equal("https://www.utpplan.com", exchange.Url);
    }
}
