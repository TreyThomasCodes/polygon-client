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
/// Unit tests for the ReferenceDataService.GetMarketStatusAsync method.
/// Tests the service's ability to retrieve market status from Polygon.io API.
/// </summary>
public class ReferenceDataService_GetMarketStatusTests
{
    private readonly Mock<IPolygonReferenceApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetMarketStatusRequest>> _mockValidator;
    private readonly Mock<ILogger<ReferenceDataService>> _mockLogger;
    private readonly ReferenceDataService _service;

    /// <summary>
    /// Initializes a new instance of the ReferenceDataService_GetMarketStatusTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public ReferenceDataService_GetMarketStatusTests()
    {
        _mockApi = new Mock<IPolygonReferenceApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetMarketStatusRequest>>();
        _mockLogger = new Mock<ILogger<ReferenceDataService>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetMarketStatusRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetMarketStatusRequest>>(), It.IsAny<CancellationToken>()))
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
    /// Tests that GetMarketStatusAsync validates the request and calls the API.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_ValidatesRequest_AndCallsApi()
    {
        // Arrange
        var request = new GetMarketStatusRequest();
        var expectedMarketStatus = new MarketStatus
        {
            Market = "open",
            AfterHours = false,
            EarlyHours = false,
            ServerTime = "2024-01-15T15:30:00Z"
        };

        _mockApi.Setup(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMarketStatus);

        // Act
        var result = await _service.GetMarketStatusAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<GetMarketStatusRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    /// <summary>
    /// Tests that GetMarketStatusAsync calls the API and returns the market status.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_CallsApi_ReturnsMarketStatus()
    {
        // Arrange
        var request = new GetMarketStatusRequest();
        var expectedMarketStatus = new MarketStatus
        {
            Market = "open",
            AfterHours = false,
            EarlyHours = false,
            ServerTime = "2024-01-15T15:30:00Z",
            Exchanges = new ExchangesStatus
            {
                NYSE = "open",
                NASDAQ = "open",
                OTC = "closed"
            },
            Currencies = new CurrencyMarketsStatus
            {
                Forex = "open",
                Crypto = "open"
            },
            IndicesGroups = new IndicesGroupsStatus
            {
                SAndP = "open",
                DowJones = "open",
                MSCI = "open",
                FTSERussell = "open",
                ICE = "open",
                SocieteGenerale = "open",
                MStar = "open"
            }
        };

        _mockApi.Setup(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMarketStatus);

        // Act
        var result = await _service.GetMarketStatusAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMarketStatus.Market, result.Market);
        Assert.Equal(expectedMarketStatus.AfterHours, result.AfterHours);
        Assert.Equal(expectedMarketStatus.EarlyHours, result.EarlyHours);
        Assert.Equal(expectedMarketStatus.ServerTime, result.ServerTime);
        Assert.NotNull(result.Exchanges);
        Assert.Equal(expectedMarketStatus.Exchanges.NYSE, result.Exchanges.NYSE);
        Assert.Equal(expectedMarketStatus.Exchanges.NASDAQ, result.Exchanges.NASDAQ);
        Assert.Equal(expectedMarketStatus.Exchanges.OTC, result.Exchanges.OTC);

        _mockApi.Verify(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetMarketStatusAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new GetMarketStatusRequest();
        var cancellationToken = new CancellationToken();
        var expectedMarketStatus = new MarketStatus { Market = "closed" };

        _mockApi.Setup(x => x.GetMarketStatusAsync(cancellationToken))
            .ReturnsAsync(expectedMarketStatus);

        // Act
        await _service.GetMarketStatusAsync(request, cancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<GetMarketStatusRequest>>(), cancellationToken), Times.Once);
        _mockApi.Verify(x => x.GetMarketStatusAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetMarketStatusAsync handles different market states correctly.
    /// </summary>
    [Theory]
    [InlineData("open", false, false)]
    [InlineData("closed", true, false)]
    [InlineData("extended-hours", false, true)]
    [InlineData("pre-market", false, true)]
    public async Task GetMarketStatusAsync_WithDifferentMarketStates_ReturnsCorrectStatus(
        string marketState, bool expectedAfterHours, bool expectedEarlyHours)
    {
        // Arrange
        var request = new GetMarketStatusRequest();
        var expectedMarketStatus = new MarketStatus
        {
            Market = marketState,
            AfterHours = expectedAfterHours,
            EarlyHours = expectedEarlyHours,
            ServerTime = "2024-01-15T15:30:00Z"
        };

        _mockApi.Setup(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMarketStatus);

        // Act
        var result = await _service.GetMarketStatusAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(marketState, result.Market);
        Assert.Equal(expectedAfterHours, result.AfterHours);
        Assert.Equal(expectedEarlyHours, result.EarlyHours);
    }

    /// <summary>
    /// Tests that GetMarketStatusAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var request = new GetMarketStatusRequest();
        _mockApi.Setup(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((MarketStatus)null!);

        // Act
        var result = await _service.GetMarketStatusAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that the service wraps API exceptions in PolygonHttpException.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var request = new GetMarketStatusRequest();
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<PolygonHttpException>(
            () => _service.GetMarketStatusAsync(request, TestContext.Current.CancellationToken));
        Assert.NotNull(actualException.InnerException);
        Assert.Equal(expectedException.Message, actualException.InnerException.Message);
    }

    /// <summary>
    /// Tests that GetMarketStatusAsync handles complex market status with all nested objects.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_WithComplexMarketStatus_ReturnsCompleteData()
    {
        // Arrange
        var request = new GetMarketStatusRequest();
        var expectedMarketStatus = new MarketStatus
        {
            Market = "open",
            AfterHours = false,
            EarlyHours = false,
            ServerTime = "2024-01-15T15:30:00Z",
            Exchanges = new ExchangesStatus
            {
                NYSE = "open",
                NASDAQ = "open",
                OTC = "closed"
            },
            Currencies = new CurrencyMarketsStatus
            {
                Forex = "open",
                Crypto = "open"
            },
            IndicesGroups = new IndicesGroupsStatus
            {
                SAndP = "open",
                DowJones = "open",
                MSCI = "open",
                FTSERussell = "open",
                ICE = "open",
                SocieteGenerale = "open",
                MStar = "open"
            }
        };

        _mockApi.Setup(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMarketStatus);

        // Act
        var result = await _service.GetMarketStatusAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMarketStatus.Market, result.Market);
        Assert.Equal(expectedMarketStatus.AfterHours, result.AfterHours);
        Assert.Equal(expectedMarketStatus.EarlyHours, result.EarlyHours);
        Assert.Equal(expectedMarketStatus.ServerTime, result.ServerTime);

        Assert.NotNull(result.Exchanges);
        Assert.Equal(expectedMarketStatus.Exchanges.NYSE, result.Exchanges.NYSE);
        Assert.Equal(expectedMarketStatus.Exchanges.NASDAQ, result.Exchanges.NASDAQ);
        Assert.Equal(expectedMarketStatus.Exchanges.OTC, result.Exchanges.OTC);

        Assert.NotNull(result.Currencies);
        Assert.Equal(expectedMarketStatus.Currencies.Forex, result.Currencies.Forex);
        Assert.Equal(expectedMarketStatus.Currencies.Crypto, result.Currencies.Crypto);

        Assert.NotNull(result.IndicesGroups);
        Assert.Equal(expectedMarketStatus.IndicesGroups.SAndP, result.IndicesGroups.SAndP);
        Assert.Equal(expectedMarketStatus.IndicesGroups.DowJones, result.IndicesGroups.DowJones);
        Assert.Equal(expectedMarketStatus.IndicesGroups.MSCI, result.IndicesGroups.MSCI);
        Assert.Equal(expectedMarketStatus.IndicesGroups.FTSERussell, result.IndicesGroups.FTSERussell);
        Assert.Equal(expectedMarketStatus.IndicesGroups.ICE, result.IndicesGroups.ICE);
        Assert.Equal(expectedMarketStatus.IndicesGroups.SocieteGenerale, result.IndicesGroups.SocieteGenerale);
        Assert.Equal(expectedMarketStatus.IndicesGroups.MStar, result.IndicesGroups.MStar);
    }
}
