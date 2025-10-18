// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.Models.Common;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the ReferenceDataService.GetConditionCodesAsync method.
/// Tests the service's ability to retrieve condition codes from Polygon.io API.
/// </summary>
public class ReferenceDataService_GetConditionCodesTests
{
    private readonly Mock<IPolygonReferenceApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetConditionCodesRequest>> _mockValidator;
    private readonly ReferenceDataService _service;

    /// <summary>
    /// Initializes a new instance of the ReferenceDataService_GetConditionCodesTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public ReferenceDataService_GetConditionCodesTests()
    {
        _mockApi = new Mock<IPolygonReferenceApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetConditionCodesRequest>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetConditionCodesRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetConditionCodesRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new ReferenceDataService(_mockApi.Object, _mockServiceProvider.Object);
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when api parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenApiIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ReferenceDataService(null!, _mockServiceProvider.Object));
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when serviceProvider parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenServiceProviderIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ReferenceDataService(_mockApi.Object, null!));
    }

    /// <summary>
    /// Tests that GetConditionCodesAsync validates the request and calls the API.
    /// </summary>
    [Fact]
    public async Task GetConditionCodesAsync_ValidatesRequest_AndCallsApi()
    {
        // Arrange
        var request = new GetConditionCodesRequest
        {
            AssetClass = AssetClass.Stocks,
            Order = SortOrder.Ascending,
            Limit = 10,
            Sort = "asset_class"
        };
        var expectedResponse = new PolygonResponse<List<ConditionCode>>
        {
            Results = new List<ConditionCode>
            {
                new ConditionCode
                {
                    Id = 1,
                    Type = "sale_condition",
                    Name = "Acquisition",
                    AssetClass = "stocks",
                    SipMapping = new SipMapping { Utp = "A" },
                    UpdateRules = new UpdateRules
                    {
                        Consolidated = new UpdateRule
                        {
                            UpdatesHighLow = true,
                            UpdatesOpenClose = true,
                            UpdatesVolume = true
                        },
                        MarketCenter = new UpdateRule
                        {
                            UpdatesHighLow = true,
                            UpdatesOpenClose = true,
                            UpdatesVolume = true
                        }
                    },
                    DataTypes = new List<string> { "trade" }
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            Count = 1
        };

        _mockApi.Setup(x => x.GetConditionCodesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<DataType?>(),
                It.IsAny<string>(),
                It.IsAny<SipMappingType?>(),
                It.IsAny<SortOrder?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetConditionCodesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<GetConditionCodesRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetConditionCodesAsync(
            request.AssetClass,
            request.DataType,
            request.Id,
            request.SipMapping,
            request.Order,
            request.Limit,
            request.Sort,
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    /// <summary>
    /// Tests that GetConditionCodesAsync calls the API and returns condition codes.
    /// </summary>
    [Fact]
    public async Task GetConditionCodesAsync_CallsApi_ReturnsConditionCodes()
    {
        // Arrange
        var request = new GetConditionCodesRequest
        {
            AssetClass = AssetClass.Stocks,
            Order = SortOrder.Ascending,
            Limit = 10,
            Sort = "asset_class"
        };
        var expectedResponse = new PolygonResponse<List<ConditionCode>>
        {
            Results = new List<ConditionCode>
            {
                new ConditionCode
                {
                    Id = 1,
                    Type = "sale_condition",
                    Name = "Acquisition",
                    AssetClass = "stocks",
                    SipMapping = new SipMapping { Utp = "A" },
                    UpdateRules = new UpdateRules
                    {
                        Consolidated = new UpdateRule
                        {
                            UpdatesHighLow = true,
                            UpdatesOpenClose = true,
                            UpdatesVolume = true
                        },
                        MarketCenter = new UpdateRule
                        {
                            UpdatesHighLow = true,
                            UpdatesOpenClose = true,
                            UpdatesVolume = true
                        }
                    },
                    DataTypes = new List<string> { "trade" }
                },
                new ConditionCode
                {
                    Id = 2,
                    Type = "sale_condition",
                    Name = "Average Price Trade",
                    AssetClass = "stocks",
                    SipMapping = new SipMapping { Cta = "B", Utp = "W", FinraTdds = "W" },
                    UpdateRules = new UpdateRules
                    {
                        Consolidated = new UpdateRule
                        {
                            UpdatesHighLow = false,
                            UpdatesOpenClose = false,
                            UpdatesVolume = true
                        },
                        MarketCenter = new UpdateRule
                        {
                            UpdatesHighLow = false,
                            UpdatesOpenClose = false,
                            UpdatesVolume = true
                        }
                    },
                    DataTypes = new List<string> { "trade" }
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            Count = 2
        };

        _mockApi.Setup(x => x.GetConditionCodesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<DataType?>(),
                It.IsAny<string>(),
                It.IsAny<SipMappingType?>(),
                It.IsAny<SortOrder?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetConditionCodesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);
        Assert.Equal("test-request-id", result.RequestId);
        Assert.Equal(2, result.Count);
        Assert.NotNull(result.Results);
        Assert.Equal(2, result.Results.Count);

        var firstCondition = result.Results[0];
        Assert.Equal(1, firstCondition.Id);
        Assert.Equal("sale_condition", firstCondition.Type);
        Assert.Equal("Acquisition", firstCondition.Name);
        Assert.Equal("stocks", firstCondition.AssetClass);
        Assert.NotNull(firstCondition.SipMapping);
        Assert.Equal("A", firstCondition.SipMapping.Utp);
        Assert.NotNull(firstCondition.UpdateRules);
        Assert.True(firstCondition.UpdateRules.Consolidated.UpdatesHighLow);
        Assert.True(firstCondition.UpdateRules.Consolidated.UpdatesOpenClose);
        Assert.True(firstCondition.UpdateRules.Consolidated.UpdatesVolume);
        Assert.Contains("trade", firstCondition.DataTypes);

        _mockApi.Verify(x => x.GetConditionCodesAsync(
            AssetClass.Stocks,
            null,
            null,
            null,
            SortOrder.Ascending,
            10,
            "asset_class",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetConditionCodesAsync passes all parameters correctly to the API.
    /// </summary>
    [Fact]
    public async Task GetConditionCodesAsync_WithAllParameters_PassesParametersToApi()
    {
        // Arrange
        var request = new GetConditionCodesRequest
        {
            AssetClass = AssetClass.Stocks,
            DataType = DataType.Trade,
            Id = "1,2,3",
            SipMapping = SipMappingType.CTA,
            Order = SortOrder.Descending,
            Limit = 100,
            Sort = "id"
        };
        var expectedResponse = new PolygonResponse<List<ConditionCode>>
        {
            Results = new List<ConditionCode>(),
            Status = "OK",
            RequestId = "test-request-id",
            Count = 0
        };

        _mockApi.Setup(x => x.GetConditionCodesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<DataType?>(),
                It.IsAny<string>(),
                It.IsAny<SipMappingType?>(),
                It.IsAny<SortOrder?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetConditionCodesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetConditionCodesAsync(
            AssetClass.Stocks,
            DataType.Trade,
            "1,2,3",
            SipMappingType.CTA,
            SortOrder.Descending,
            100,
            "id",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetConditionCodesAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetConditionCodesAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new GetConditionCodesRequest();
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<List<ConditionCode>>
        {
            Results = new List<ConditionCode>(),
            Status = "OK",
            RequestId = "test-request-id",
            Count = 0
        };

        _mockApi.Setup(x => x.GetConditionCodesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<DataType?>(),
                It.IsAny<string>(),
                It.IsAny<SipMappingType?>(),
                It.IsAny<SortOrder?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetConditionCodesAsync(request, cancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<GetConditionCodesRequest>>(), cancellationToken), Times.Once);
        _mockApi.Verify(x => x.GetConditionCodesAsync(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetConditionCodesAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetConditionCodesAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var request = new GetConditionCodesRequest();
        _mockApi.Setup(x => x.GetConditionCodesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<DataType?>(),
                It.IsAny<string>(),
                It.IsAny<SipMappingType?>(),
                It.IsAny<SortOrder?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<List<ConditionCode>>)null!);

        // Act
        var result = await _service.GetConditionCodesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetConditionCodesAsync(
            It.IsAny<AssetClass?>(),
            It.IsAny<DataType?>(),
            It.IsAny<string>(),
            It.IsAny<SipMappingType?>(),
            It.IsAny<SortOrder?>(),
            It.IsAny<int?>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetConditionCodesAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetConditionCodesAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var request = new GetConditionCodesRequest();
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetConditionCodesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<DataType?>(),
                It.IsAny<string>(),
                It.IsAny<SipMappingType?>(),
                It.IsAny<SortOrder?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetConditionCodesAsync(request, TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetConditionCodesAsync handles condition codes with legacy flag.
    /// </summary>
    [Fact]
    public async Task GetConditionCodesAsync_WithLegacyConditions_ReturnsCorrectData()
    {
        // Arrange
        var request = new GetConditionCodesRequest();
        var expectedResponse = new PolygonResponse<List<ConditionCode>>
        {
            Results = new List<ConditionCode>
            {
                new ConditionCode
                {
                    Id = 6,
                    Type = "sale_condition",
                    Name = "CAP Election",
                    AssetClass = "stocks",
                    SipMapping = new SipMapping { Cta = "I" },
                    UpdateRules = new UpdateRules
                    {
                        Consolidated = new UpdateRule
                        {
                            UpdatesHighLow = true,
                            UpdatesOpenClose = true,
                            UpdatesVolume = true
                        },
                        MarketCenter = new UpdateRule
                        {
                            UpdatesHighLow = true,
                            UpdatesOpenClose = true,
                            UpdatesVolume = true
                        }
                    },
                    DataTypes = new List<string> { "trade" },
                    Legacy = true
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            Count = 1
        };

        _mockApi.Setup(x => x.GetConditionCodesAsync(
                It.IsAny<AssetClass?>(),
                It.IsAny<DataType?>(),
                It.IsAny<string>(),
                It.IsAny<SipMappingType?>(),
                It.IsAny<SortOrder?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetConditionCodesAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Single(result.Results);

        var condition = result.Results[0];
        Assert.Equal(6, condition.Id);
        Assert.Equal("CAP Election", condition.Name);
        Assert.True(condition.Legacy);
        Assert.Equal("I", condition.SipMapping.Cta);
    }
}
