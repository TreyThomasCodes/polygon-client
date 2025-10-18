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
/// Unit tests for the OptionsService.GetDailyOpenCloseAsync method.
/// Tests the service's ability to retrieve data from Polygon.io API.
/// </summary>
public class OptionsService_GetDailyOpenCloseTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetDailyOpenCloseRequest>> _mockValidator;
    private readonly Mock<ILogger<OptionsService>> _mockLogger;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetDailyOpenCloseTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public OptionsService_GetDailyOpenCloseTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetDailyOpenCloseRequest>>();
        _mockLogger = new Mock<ILogger<OptionsService>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetDailyOpenCloseRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetDailyOpenCloseRequest>>(), It.IsAny<CancellationToken>()))
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
    /// Tests that GetDailyOpenCloseAsync calls the API and returns the daily open/close response.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_CallsApi_ReturnsDailyOpenCloseResponse()
    {
        // Arrange
        var request = new GetDailyOpenCloseRequest
        {
            OptionsTicker = "O:SPY251219C00650000",
            Date = "2023-01-09"
        };
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
                request.OptionsTicker,
                request.Date,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetDailyOpenCloseAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetDailyOpenCloseRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
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
            request.OptionsTicker,
            request.Date,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetDailyOpenCloseAsync passes parameters correctly to the API.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_PassesParametersToApi()
    {
        // Arrange
        var request = new GetDailyOpenCloseRequest
        {
            OptionsTicker = "O:TSLA260320C00700000",
            Date = "2023-03-20"
        };
        var expectedResponse = new OptionDailyOpenClose();

        _mockApi
            .Setup(x => x.GetDailyOpenCloseAsync(
                request.OptionsTicker,
                request.Date,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetDailyOpenCloseAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetDailyOpenCloseRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetDailyOpenCloseAsync(
            request.OptionsTicker,
            request.Date,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetDailyOpenCloseAsync handles null values correctly.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_HandlesNullValues()
    {
        // Arrange
        var request = new GetDailyOpenCloseRequest
        {
            OptionsTicker = "O:SPY251219C00650000",
            Date = "2023-01-09"
        };
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
                request.OptionsTicker,
                request.Date,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetDailyOpenCloseAsync(request, TestContext.Current.CancellationToken);

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

    /// <summary>
    /// Tests that GetDailyOpenCloseAsync throws PolygonValidationException when request is invalid.
    /// </summary>
    [Fact]
    public async Task GetDailyOpenCloseAsync_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new GetDailyOpenCloseRequest { OptionsTicker = "", Date = "" };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("OptionsTicker", "Options ticker must not be empty."),
            new ValidationFailure("Date", "Date must not be empty.")
        };
        var validationResult = new ValidationResult(validationFailures);

        // Reset and setup the validator to throw PolygonValidationException directly
        _mockValidator.Reset();
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetDailyOpenCloseRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<PolygonValidationException>(
            () => _service.GetDailyOpenCloseAsync(request, TestContext.Current.CancellationToken));

        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetDailyOpenCloseRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetDailyOpenCloseAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
