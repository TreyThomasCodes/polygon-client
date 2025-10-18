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
/// Unit tests for the StocksService.GetLastQuoteAsync method.
/// Tests the service's ability to retrieve last quote data from Polygon.io API.
/// </summary>
public class StocksService_GetLastQuoteTests
{
    private readonly Mock<IPolygonStocksApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetLastQuoteRequest>> _mockValidator;
    private readonly Mock<ILogger<StocksService>> _mockLogger;
    private readonly StocksService _service;

    /// <summary>
    /// Initializes a new instance of the StocksService_GetLastQuoteTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public StocksService_GetLastQuoteTests()
    {
        _mockApi = new Mock<IPolygonStocksApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetLastQuoteRequest>>();
        _mockLogger = new Mock<ILogger<StocksService>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetLastQuoteRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetLastQuoteRequest>>(), It.IsAny<CancellationToken>()))
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
    /// Tests that GetLastQuoteAsync validates the request and calls the API.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_ValidatesRequest_AndCallsApi()
    {
        // Arrange
        var request = new GetLastQuoteRequest
        {
            Ticker = "AAPL"
        };
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Results = new LastQuoteResult
            {
                Ticker = "AAPL",
                BidPrice = 254.05m,
                AskPrice = 254m
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastQuoteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetLastQuoteRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetLastQuoteAsync(request.Ticker, It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync calls the API and returns the last quote response.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_CallsApi_ReturnsLastQuoteResponse()
    {
        // Arrange
        var request = new GetLastQuoteRequest
        {
            Ticker = "AAPL"
        };
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Results = new LastQuoteResult
            {
                Ticker = "AAPL",
                BidPrice = 254.05m,
                BidSize = 2,
                AskPrice = 254m,
                AskSize = 6,
                BidExchange = 12,
                AskExchange = 8,
                Tape = 3,
                Timestamp = 1759266523134155000L,
                ParticipantTimestamp = 1759266523134142700L,
                Sequence = 69066304L,
                Indicators = new List<int> { 604 }
            },
            Status = "OK",
            RequestId = "232e88d413f65da04b9bcd6063adbfcb"
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastQuoteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.BidPrice, result.Results.BidPrice);
        Assert.Equal(expectedResponse.Results.BidSize, result.Results.BidSize);
        Assert.Equal(expectedResponse.Results.AskPrice, result.Results.AskPrice);
        Assert.Equal(expectedResponse.Results.AskSize, result.Results.AskSize);
        Assert.Equal(expectedResponse.Results.BidExchange, result.Results.BidExchange);
        Assert.Equal(expectedResponse.Results.AskExchange, result.Results.AskExchange);
        Assert.Equal(expectedResponse.Results.Tape, result.Results.Tape);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.ParticipantTimestamp, result.Results.ParticipantTimestamp);
        Assert.Equal(expectedResponse.Results.Sequence, result.Results.Sequence);
        Assert.Equal(expectedResponse.Results.Indicators, result.Results.Indicators);

        _mockApi.Verify(x => x.GetLastQuoteAsync(request.Ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new GetLastQuoteRequest
        {
            Ticker = "MSFT"
        };
        var cancellationToken = new CancellationToken();
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Status = "OK",
            Results = new LastQuoteResult { Ticker = request.Ticker }
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(request.Ticker, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetLastQuoteAsync(request, cancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetLastQuoteRequest>>(ctx => ctx.InstanceToValidate == request), cancellationToken), Times.Once);
        _mockApi.Verify(x => x.GetLastQuoteAsync(request.Ticker, cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync handles different ticker symbols correctly.
    /// </summary>
    [Theory]
    [InlineData("AAPL")]
    [InlineData("MSFT")]
    [InlineData("TSLA")]
    [InlineData("GOOGL")]
    public async Task GetLastQuoteAsync_WithDifferentTickers_CallsApiWithCorrectTicker(string ticker)
    {
        // Arrange
        var request = new GetLastQuoteRequest
        {
            Ticker = ticker
        };
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Results = new LastQuoteResult
            {
                Ticker = ticker,
                BidPrice = 100.00m,
                AskPrice = 100.10m
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastQuoteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticker, result.Results?.Ticker);
        _mockApi.Verify(x => x.GetLastQuoteAsync(ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var request = new GetLastQuoteRequest
        {
            Ticker = "INVALID"
        };
        _mockApi.Setup(x => x.GetLastQuoteAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Common.PolygonResponse<LastQuoteResult>)null!);

        // Act
        var result = await _service.GetLastQuoteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetLastQuoteAsync(request.Ticker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that the service wraps API exceptions in PolygonHttpException.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var request = new GetLastQuoteRequest
        {
            Ticker = "AAPL"
        };
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetLastQuoteAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<PolygonHttpException>(
            () => _service.GetLastQuoteAsync(request, TestContext.Current.CancellationToken));
        Assert.NotNull(actualException.InnerException);
        Assert.Equal(expectedException.Message, actualException.InnerException.Message);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync throws PolygonValidationException when request is invalid.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new GetLastQuoteRequest
        {
            Ticker = ""
        };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Ticker", "Ticker must not be empty.")
        };

        _mockValidator.Reset();
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetLastQuoteRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<PolygonValidationException>(
            () => _service.GetLastQuoteAsync(request, TestContext.Current.CancellationToken));

        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetLastQuoteRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetLastQuoteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var request = new GetLastQuoteRequest
        {
            Ticker = "AAPL"
        };
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastQuoteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Results);
    }

    /// <summary>
    /// Tests that GetLastQuoteAsync handles complete quote data with all fields populated.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_WithCompleteQuoteData_ReturnsCompleteData()
    {
        // Arrange
        var request = new GetLastQuoteRequest
        {
            Ticker = "AAPL"
        };
        var expectedResponse = new Models.Common.PolygonResponse<LastQuoteResult>
        {
            Results = new LastQuoteResult
            {
                Ticker = "AAPL",
                BidPrice = 254.05m,
                BidSize = 2,
                AskPrice = 254m,
                AskSize = 6,
                BidExchange = 12,
                AskExchange = 8,
                Tape = 3,
                Timestamp = 1759266523134155000L,
                ParticipantTimestamp = 1759266523134142700L,
                Sequence = 69066304L,
                Indicators = new List<int> { 604 }
            },
            Status = "OK",
            RequestId = "232e88d413f65da04b9bcd6063adbfcb"
        };

        _mockApi.Setup(x => x.GetLastQuoteAsync(request.Ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastQuoteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);

        Assert.NotNull(result.Results);
        Assert.Equal(expectedResponse.Results.Ticker, result.Results.Ticker);
        Assert.Equal(expectedResponse.Results.BidPrice, result.Results.BidPrice);
        Assert.Equal(expectedResponse.Results.BidSize, result.Results.BidSize);
        Assert.Equal(expectedResponse.Results.AskPrice, result.Results.AskPrice);
        Assert.Equal(expectedResponse.Results.AskSize, result.Results.AskSize);
        Assert.Equal(expectedResponse.Results.BidExchange, result.Results.BidExchange);
        Assert.Equal(expectedResponse.Results.AskExchange, result.Results.AskExchange);
        Assert.Equal(expectedResponse.Results.Tape, result.Results.Tape);
        Assert.Equal(expectedResponse.Results.Timestamp, result.Results.Timestamp);
        Assert.Equal(expectedResponse.Results.ParticipantTimestamp, result.Results.ParticipantTimestamp);
        Assert.Equal(expectedResponse.Results.Sequence, result.Results.Sequence);
        Assert.Equal(expectedResponse.Results.Indicators, result.Results.Indicators);
    }
}
