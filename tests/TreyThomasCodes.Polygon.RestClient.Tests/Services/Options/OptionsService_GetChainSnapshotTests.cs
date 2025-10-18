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
/// Unit tests for the OptionsService.GetChainSnapshotAsync method.
/// Tests the service's ability to retrieve options chain snapshot data from Polygon.io API.
/// </summary>
public class OptionsService_GetChainSnapshotTests
{
    private readonly Mock<IPolygonOptionsApi> _mockApi;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IValidator<GetChainSnapshotRequest>> _mockValidator;
    private readonly OptionsService _service;

    /// <summary>
    /// Initializes a new instance of the OptionsService_GetChainSnapshotTests class.
    /// Sets up the mock API, service provider, validator, and service instance for testing.
    /// </summary>
    public OptionsService_GetChainSnapshotTests()
    {
        _mockApi = new Mock<IPolygonOptionsApi>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockValidator = new Mock<IValidator<GetChainSnapshotRequest>>();

        // Setup service provider to return the validator
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<GetChainSnapshotRequest>)))
            .Returns(_mockValidator.Object);

        // Setup validator to return success by default
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetChainSnapshotRequest>>(), It.IsAny<CancellationToken>()))
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
    /// Tests that GetChainSnapshotAsync calls the API and returns the chain snapshot response.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_CallsApi_ReturnsChainSnapshotResponse()
    {
        // Arrange
        var request = new GetChainSnapshotRequest { UnderlyingAsset = "MSTR" };
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    BreakEvenPrice = 310.85m,
                    Day = new OptionDayData
                    {
                        Change = 0m,
                        ChangePercent = 0m,
                        Close = 172.65m,
                        High = 172.65m,
                        LastUpdated = 1760040000000000000,
                        Low = 172.65m,
                        Open = 172.65m,
                        PreviousClose = 172.65m,
                        Volume = 5,
                        Vwap = 172.65m
                    },
                    Details = new OptionContractDetails
                    {
                        ContractType = "call",
                        ExerciseStyle = "american",
                        ExpirationDate = "2025-10-10",
                        SharesPerContract = 100,
                        StrikePrice = 150,
                        Ticker = "O:MSTR251010C00150000"
                    },
                    Greeks = new OptionGreeks
                    {
                        Delta = 0.9951282876879873m,
                        Gamma = 0.00014861413681600426m,
                        Theta = -2.053186766077909m,
                        Vega = 0.0013923356112067263m
                    },
                    ImpliedVolatility = 10.17106034370156m,
                    LastQuote = new OptionLastQuote
                    {
                        Ask = 161.85m,
                        AskSize = 13,
                        AskExchange = 301,
                        Bid = 159.85m,
                        BidSize = 13,
                        BidExchange = 301,
                        LastUpdated = 1760112568871916300,
                        Midpoint = 160.85m,
                        Timeframe = "REAL-TIME"
                    },
                    LastTrade = new OptionLastTrade
                    {
                        SipTimestamp = 1760020832057391000,
                        Conditions = new List<int> { 231 },
                        Price = 172.65m,
                        Size = 5,
                        Exchange = 301,
                        Timeframe = "REAL-TIME"
                    },
                    OpenInterest = 47,
                    UnderlyingAsset = new OptionUnderlyingAsset
                    {
                        ChangeToBreakEven = 0.17m,
                        LastUpdated = 1760112614961306000,
                        Price = 310.68m,
                        Ticker = "MSTR",
                        Timeframe = "REAL-TIME"
                    }
                },
                new OptionSnapshot
                {
                    BreakEvenPrice = 311.125m,
                    Details = new OptionContractDetails
                    {
                        ContractType = "call",
                        ExerciseStyle = "american",
                        ExpirationDate = "2025-10-10",
                        SharesPerContract = 100,
                        StrikePrice = 160,
                        Ticker = "O:MSTR251010C00160000"
                    },
                    OpenInterest = 9
                }
            },
            Status = "OK",
            RequestId = "da2e6e0f4063e80346179d18ca3a5fd8",
            NextUrl = "https://api.polygon.io/v3/snapshot/options/MSTR?cursor=YXA9TyUzQU1TVFIyNTEwMTBDMDAyMTI1MDAlM0EyMTIuNTAmYXM9JmV4cGlyYXRpb25fZGF0ZS5ndGU9MjAyNS0xMC0xMCZsaW1pdD0xMCZvcmRlcj1hc2Mmc29ydD10aWNrZXI"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetChainSnapshotRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Equal(expectedResponse.NextUrl, result.NextUrl);
        Assert.NotNull(result.Results);
        Assert.Equal(2, result.Results.Count);
        Assert.Equal(310.85m, result.Results[0].BreakEvenPrice);
        Assert.Equal("O:MSTR251010C00150000", result.Results[0].Details?.Ticker);
        Assert.Equal("O:MSTR251010C00160000", result.Results[1].Details?.Ticker);

        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync passes all parameters correctly to the API.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithAllParameters_PassesParametersCorrectly()
    {
        // Arrange
        var request = new GetChainSnapshotRequest
        {
            UnderlyingAsset = "SPY",
            StrikePrice = 650m,
            ContractType = "call",
            ExpirationDateGte = "2025-01-01",
            ExpirationDateLte = "2025-12-31",
            Limit = 10,
            Order = "asc",
            Sort = "strike_price",
            Cursor = "test-cursor"
        };
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>(),
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            request.StrikePrice,
            request.ContractType,
            request.ExpirationDateGte,
            request.ExpirationDateLte,
            request.Limit,
            request.Order,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetChainSnapshotRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            request.StrikePrice,
            request.ContractType,
            request.ExpirationDateGte,
            request.ExpirationDateLte,
            request.Limit,
            request.Order,
            request.Sort,
            request.Cursor,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new GetChainSnapshotRequest { UnderlyingAsset = "SPY" };
        var cancellationToken = new CancellationToken();
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>> { Status = "OK" };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetChainSnapshotAsync(request, cancellationToken);

        // Assert
        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetChainSnapshotRequest>>(ctx => ctx.InstanceToValidate == request), cancellationToken), Times.Once);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles different underlying assets correctly.
    /// </summary>
    [Theory]
    [InlineData("SPY")]
    [InlineData("AAPL")]
    [InlineData("TSLA")]
    [InlineData("MSTR")]
    public async Task GetChainSnapshotAsync_WithDifferentUnderlyingAssets_CallsApiWithCorrectAsset(string underlyingAsset)
    {
        // Arrange
        var request = new GetChainSnapshotRequest { UnderlyingAsset = underlyingAsset };
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    UnderlyingAsset = new OptionUnderlyingAsset
                    {
                        Ticker = underlyingAsset
                    }
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(underlyingAsset, result.Results?[0].UnderlyingAsset?.Ticker);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            underlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        var request = new GetChainSnapshotRequest { UnderlyingAsset = "INVALID" };
        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((PolygonResponse<List<OptionSnapshot>>)null!);

        // Act
        var result = await _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var request = new GetChainSnapshotRequest { UnderlyingAsset = "SPY" };
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles response with null results.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithNullResults_ReturnsResponseWithNullResults()
    {
        // Arrange
        var request = new GetChainSnapshotRequest { UnderlyingAsset = "SPY" };
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = null,
            Status = "NOT_FOUND",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.Null(result.Results);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles empty results list.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithEmptyResults_ReturnsEmptyList()
    {
        // Arrange
        var request = new GetChainSnapshotRequest { UnderlyingAsset = "SPY" };
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>(),
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Empty(result.Results);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync with strike price filter passes the parameter correctly.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithStrikePrice_PassesStrikePriceCorrectly()
    {
        // Arrange
        var request = new GetChainSnapshotRequest
        {
            UnderlyingAsset = "SPY",
            StrikePrice = 650m
        };
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    Details = new OptionContractDetails
                    {
                        StrikePrice = 650m
                    }
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            request.StrikePrice,
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(650m, result.Results?[0].Details?.StrikePrice);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            request.StrikePrice,
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync with contract type filter returns only the specified type.
    /// </summary>
    [Theory]
    [InlineData("call")]
    [InlineData("put")]
    public async Task GetChainSnapshotAsync_WithContractType_FiltersCorrectly(string contractType)
    {
        // Arrange
        var request = new GetChainSnapshotRequest
        {
            UnderlyingAsset = "SPY",
            ContractType = contractType
        };
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    Details = new OptionContractDetails
                    {
                        ContractType = contractType
                    }
                }
            },
            Status = "OK"
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            contractType,
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contractType, result.Results?[0].Details?.ContractType);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            contractType,
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync handles pagination with next_url correctly.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithPagination_ReturnsNextUrl()
    {
        // Arrange
        var request = new GetChainSnapshotRequest { UnderlyingAsset = "MSTR" };
        var nextUrl = "https://api.polygon.io/v3/snapshot/options/MSTR?cursor=test-cursor";
        var expectedResponse = new PolygonResponse<List<OptionSnapshot>>
        {
            Results = new List<OptionSnapshot>
            {
                new OptionSnapshot
                {
                    Details = new OptionContractDetails
                    {
                        Ticker = "O:MSTR251010C00150000"
                    }
                }
            },
            Status = "OK",
            RequestId = "test-request-id",
            NextUrl = nextUrl
        };

        _mockApi.Setup(x => x.GetChainSnapshotAsync(
            request.UnderlyingAsset,
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nextUrl, result.NextUrl);
    }

    /// <summary>
    /// Tests that GetChainSnapshotAsync throws ValidationException when request is invalid.
    /// </summary>
    [Fact]
    public async Task GetChainSnapshotAsync_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new GetChainSnapshotRequest { UnderlyingAsset = "" };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("UnderlyingAsset", "Underlying asset must not be empty.")
        };
        var validationResult = new ValidationResult(validationFailures);

        // Reset and setup the validator to throw ValidationException directly
        _mockValidator.Reset();
        _mockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<GetChainSnapshotRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.GetChainSnapshotAsync(request, TestContext.Current.CancellationToken));

        _mockValidator.Verify(x => x.ValidateAsync(It.Is<ValidationContext<GetChainSnapshotRequest>>(ctx => ctx.InstanceToValidate == request), It.IsAny<CancellationToken>()), Times.Once);
        _mockApi.Verify(x => x.GetChainSnapshotAsync(
            It.IsAny<string>(),
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
