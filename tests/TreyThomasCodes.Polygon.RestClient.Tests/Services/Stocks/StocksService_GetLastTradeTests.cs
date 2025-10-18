// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Exceptions;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the StocksService.GetLastTradeAsync method.
/// Tests the service's ability to retrieve last trade data from Polygon.io API.
/// </summary>
public class StocksService_GetLastTradeTests
{
    private readonly Mock<IPolygonStocksApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetLastTradeRequest>> _mockValidator;
    private readonly Mock<ILogger<StocksService>> _mockLogger;
    private readonly StocksService _service;

    /// <summary>
    /// Initializes a new instance of the StocksService_GetLastTradeTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public StocksService_GetLastTradeTests()
    {
        _mockApi = new Mock<IPolygonStocksApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetLastTradeRequest>>();
        _mockLogger = new Mock<ILogger<StocksService>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetLastTradeRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetLastTradeRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new StocksService(_mockApi.Object, _mockServiceProvider.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when api parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenApiIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new StocksService(null!, _mockServiceProvider.Object, _mockLogger.Object));
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when serviceProvider parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenServiceProviderIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new StocksService(_mockApi.Object, null!, _mockLogger.Object));
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when logger parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new StocksService(_mockApi.Object, _mockServiceProvider.Object, null!));
    }

    /// <summary>
    /// Tests that GetLastTradeAsync validates the request and calls the API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_ValidatesRequest_AndCallsApi()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            Ticker = "AAPL"
        };
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Results = new StockTrade
            {
                Ticker = "AAPL",
                Price = 254.11m,
                Size = 5
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetLastTradeRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetLastTradeAsync(request.Ticker, It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync calls the API and returns the last trade response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_CallsApi_ReturnsLastTradeResponse()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            Ticker = "AAPL"
        };
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Results = new StockTrade
            {
                Ticker = "AAPL",
                Conditions = new List<int> { 12, 37 },
                TimeframeStart = 1759264235449251800,
                Id = "200627",
                Price = 254.11m,
                Sequence = 9841657,
                TapeOrCorrection = 202,
                Size = 5,
                Timestamp = 1759264235449270800,
                Exchange = 4,
                ParticipantTimestamp = 1759264235447000000
            },
            Status = "OK",
            RequestId = "7453d68e76e95db17f29fbde6e5cce4d"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.Price, result.Results.Price);
        Assert.Equal(expectedResponse.Results.Size, result.Results.Size);
        Assert.Equal(expectedResponse.Results.Exchange, result.Results.Exchange);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.Id, result.Results.Id);
        Assert.Equal(expectedResponse.Results.Conditions, result.Results.Conditions);

        _mockApi.Verify(x => x.GetLastTradeAsync(request.Ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            Ticker = "MSFT"
        };
        var cancellationToken = new CancellationToken();
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Status = "OK",
            Results = new StockTrade { Ticker = request.Ticker }
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.Ticker, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetLastTradeAsync(request, cancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetLastTradeRequest>>(ctx => ctx.InstanceToValidate == request), cancellationToken), Times.Once);
        _mockApi.Verify(x => x.GetLastTradeAsync(request.Ticker, cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles different ticker symbols correctly.
    /// </summary>
    [Theory]
    [InlineData("AAPL")]
    [InlineData("MSFT")]
    [InlineData("TSLA")]
    [InlineData("GOOGL")]
    public async Task GetLastTradeAsync_WithDifferentTickers_CallsApiWithCorrectTicker(string ticker)
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            Ticker = ticker
        };
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Results = new StockTrade
            {
                Ticker = ticker,
                Price = 100.00m,
                Size = 50
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticker, result.Results?.Ticker);
        _mockApi.Verify(x => x.GetLastTradeAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            Ticker = "INVALID"
        };
        _mockApi.Setup(x => x.GetLastTradeAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Common.PolygonResponse<StockTrade>)null!);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetLastTradeAsync(request.Ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that the service wraps API exceptions in PolygonHttpException.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            Ticker = "AAPL"
        };
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetLastTradeAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<PolygonHttpException>(
            () => _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken));
        Assert.NotNull(actualException.InnerException);
        Assert.Equal(expectedException.Message, actualException.InnerException.Message);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync throws PolygonValidationException when request is invalid.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            Ticker = ""
        };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Ticker", "Ticker must not be empty.")
        };

        _mockValidator.Reset();
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetLastTradeRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<PolygonValidationException>(
            () => _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken));

        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetLastTradeRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetLastTradeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var request = new GetLastTradeRequest
        {
            Ticker = "AAPL"
        };
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetLastTradeRequest
        {
            Ticker = "AAPL"
        };
        var expectedResponse = new Models.Common.PolygonResponse<StockTrade>
        {
            Results = new StockTrade
            {
                Ticker = "AAPL",
                Timestamp = 1759264235449270800,
                TimeframeStart = 1759264235449251800,
                Sequence = 9841657,
                ParticipantTimestamp = 1759264235447000000,
                Conditions = new List<int> { 12, 37 },
                Id = "200627",
                Exchange = 4,
                Price = 254.11m,
                Size = 5,
                TapeOrCorrection = 202
            },
            Status = "OK",
            RequestId = "7453d68e76e95db17f29fbde6e5cce4d"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.TimeframeStart, result.Results.TimeframeStart);
        Assert.Equal(expectedResponse.Results.Sequence, result.Results.Sequence);
        Assert.Equal(expectedResponse.Results.ParticipantTimestamp, result.Results.ParticipantTimestamp);
        Assert.Equal(expectedResponse.Results.Conditions, result.Results.Conditions);
        Assert.Equal(expectedResponse.Results.Id, result.Results.Id);
        Assert.Equal(expectedResponse.Results.Exchange, result.Results.Exchange);
        Assert.Equal(expectedResponse.Results.Price, result.Results.Price);
        Assert.Equal(expectedResponse.Results.Size, result.Results.Size);
        Assert.Equal(expectedResponse.Results.TapeOrCorrection, result.Results.TapeOrCorrection);
    }
}
