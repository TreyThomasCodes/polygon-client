// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services.Options;

/// <summary>
/// Unit tests for the OptionsService.GetLastTradeAsync method.
/// Tests the service's ability to retrieve data from Polygon.io API.
/// </summary>
public class OptionsService_GetLastTradeTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetLastTradeRequest>> _mockValidator;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetLastTradeTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public OptionsService_GetLastTradeTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetLastTradeRequest>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetLastTradeRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetLastTradeRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new OptionsService(_mockApi.Object, _mockServiceProvider.Object);
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when serviceProvider parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenServiceProviderIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OptionsService(_mockApi.Object, null!));
    }

    /// <summary>
    /// Tests that GetLastTradeAsync calls the API and returns the last trade response.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_CallsApi_ReturnsLastTradeResponse()
    {
        // Arrange
        var request = new GetLastTradeRequest { OptionsTicker = "O:TSLA260320C00700000" };
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

        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetLastTradeRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
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

        _mockApi.Verify(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new GetLastTradeRequest { OptionsTicker = "O:TSLA260320C00700000" };
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<OptionTrade> { Status = "OK" };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetLastTradeAsync(request, cancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetLastTradeRequest>>(ctx => ctx.InstanceToValidate == request), cancellationToken), Times.Once);
        _mockApi.Verify(x => x.GetLastTradeAsync(request.OptionsTicker, cancellationToken), Times.Once);
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
        var request = new GetLastTradeRequest { OptionsTicker = optionsTicker };
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
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetLastTradeRequest { OptionsTicker = "O:INVALID000000C00000000" };
        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<OptionTrade>)null!);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var request = new GetLastTradeRequest { OptionsTicker = "O:SPY251219C00650000" };
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var request = new GetLastTradeRequest { OptionsTicker = "O:SPY251219C00650000" };
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
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
        var request = new GetLastTradeRequest { OptionsTicker = "O:TSLA260320C00700000" };
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

        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetLastTradeRequest { OptionsTicker = "O:SPY251219P00650000" };
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = request.OptionsTicker,
                Price = 8.75m,
                Size = 3,
                Timestamp = 1760121506853685800,
                Exchange = 312
            },
            Status = "OK",
            RequestId = "test-put-request-id"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal(request.OptionsTicker, result.Results.Ticker);
        Assert.Contains('P', request.OptionsTicker);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync handles responses with minimal data correctly.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithMinimalData_ReturnsPartialTrade()
    {
        // Arrange
        var request = new GetLastTradeRequest { OptionsTicker = "O:SPY251219C00650000" };
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

        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
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
        var request = new GetLastTradeRequest { OptionsTicker = "O:AAPL250117C00150000" };
        var conditions = new List<int> { 227, 228, 229 };
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = request.OptionsTicker,
                Conditions = conditions,
                Price = 5.25m,
                Size = 10,
                Timestamp = 1760121506853685800,
                Exchange = 301
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

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
        var request = new GetLastTradeRequest { OptionsTicker = "O:SPY251219C00650000" };
        var timestamp = 1760121506853685800;
        var expectedResponse = new PolygonResponse<OptionTrade>
        {
            Results = new OptionTrade
            {
                Ticker = request.OptionsTicker,
                Timestamp = timestamp,
                Price = 12.3m
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetLastTradeAsync(request.OptionsTicker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Equal(timestamp, result.Results.Timestamp);
        Assert.NotNull(result.Results.MarketTimestamp);
        Assert.Equal("America/New_York", result.Results.MarketTimestamp.Value.Zone.Id);
    }

    /// <summary>
    /// Tests that GetLastTradeAsync throws ValidationException when request is invalid.
    /// </summary>
    [Fact]
    public async Task GetLastTradeAsync_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new GetLastTradeRequest { OptionsTicker = "" };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("OptionsTicker", "Options ticker must not be empty.")
        };
        var validationResult = new ValidationResult(validationFailures);

        // Reset and setup the validator to throw ValidationException directly
        _mockValidator.Reset();
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetLastTradeRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.GetLastTradeAsync(request, TestContext.Current.CancellationToken));

        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetLastTradeRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetLastTradeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
