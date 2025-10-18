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
/// Unit tests for the OptionsService.GetQuotesAsync method.
/// Tests the service's ability to retrieve data from Polygon.io API.
/// </summary>
public class OptionsService_GetQuotesTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetQuotesRequest>> _mockValidator;
    private readonly Mock<ILogger<OptionsService>> _mockLogger;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetQuotesTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public OptionsService_GetQuotesTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetQuotesRequest>>();
        _mockLogger = new Mock<ILogger<OptionsService>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetQuotesRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetQuotesRequest>>(), It.IsAny<CancellationToken>()))
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
    /// Tests that GetQuotesAsync validates the request and calls the API.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_ValidatesRequest_AndCallsApi()
    {
        // Arrange
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote> { new OptionQuote { AskPrice = 294.98m } },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetQuotesRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    /// <summary>
    /// Tests that GetQuotesAsync calls the API and returns the quotes response.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_CallsApi_ReturnsQuotesResponse()
    {
        // Arrange
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
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
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

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
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetQuotesAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<List<OptionQuote>> { Status = "OK" };

        _mockApi.Setup(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetQuotesAsync(request, cancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetQuotesRequest>>(ctx => ctx.InstanceToValidate == request), cancellationToken), Times.Once);
        _mockApi.Verify(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetQuotesAsync passes all parameters correctly to the API.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithAllParameters_PassesParametersCorrectly()
    {
        // Arrange
        var request = new GetQuotesRequest
        {
            OptionsTicker = "O:SPY241220P00720000",
            Timestamp = "2022-03-07",
            TimestampLt = "2022-03-08",
            TimestampLte = "2022-03-07T23:59:59",
            TimestampGt = "2022-03-06",
            TimestampGte = "2022-03-07T00:00:00",
            Order = "asc",
            Limit = 10,
            Sort = "timestamp",
            Cursor = "test-cursor"
        };
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>(),
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockApi.Verify(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
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
        var request = new GetQuotesRequest { OptionsTicker = optionsTicker };
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
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockApi.Verify(x => x.GetQuotesAsync(
            optionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetQuotesAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var request = new GetQuotesRequest { OptionsTicker = "O:INVALID000000C00000000" };
        _mockApi.Setup(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<List<OptionQuote>>)null!);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that the service wraps API exceptions in PolygonHttpException.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<PolygonHttpException>(
            () => _service.GetQuotesAsync(request, TestContext.Current.CancellationToken));
        Assert.NotNull(actualException.InnerException);
        Assert.Equal(expectedException.Message, actualException.InnerException.Message);
    }

    /// <summary>
    /// Tests that GetQuotesAsync throws PolygonValidationException when request is invalid.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new GetQuotesRequest { OptionsTicker = "" };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("OptionsTicker", "Options ticker must not be empty.")
        };
        var validationResult = new ValidationResult(validationFailures);

        // Reset and setup the validator to throw PolygonValidationException directly
        _mockValidator.Reset();
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetQuotesRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<PolygonValidationException>(
            () => _service.GetQuotesAsync(request, TestContext.Current.CancellationToken));

        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetQuotesRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetQuotesAsync(
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Tests that GetQuotesAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
        var expectedResponse = new PolygonResponse<List<OptionQuote>>
        {
            Results = new List<OptionQuote>(),
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
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
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetQuotesRequest
        {
            OptionsTicker = "O:SPY241220P00720000",
            Timestamp = "2022-03-07T14:30:00"
        };
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
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockApi.Verify(x => x.GetQuotesAsync(
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetQuotesAsync MarketTimestamp property converts timestamp correctly.
    /// </summary>
    [Fact]
    public async Task GetQuotesAsync_MarketTimestamp_ConvertsToEasternTime()
    {
        // Arrange
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
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
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
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
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetQuotesRequest { OptionsTicker = "O:SPY241220P00720000" };
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
            request.OptionsTicker,
            request.Timestamp,
            request.TimestampLt,
            request.TimestampLte,
            request.TimestampGt,
            request.TimestampGte,
            request.Order,
            request.Limit,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetQuotesAsync(request, TestContext.Current.CancellationToken);

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
}
