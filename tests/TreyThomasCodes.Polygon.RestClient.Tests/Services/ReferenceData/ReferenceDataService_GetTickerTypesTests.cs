// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Moq;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.Models.Common;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the ReferenceDataService.GetTickerTypesAsync method.
/// Tests the service's ability to retrieve ticker types from Polygon.io API.
/// </summary>
public class ReferenceDataService_GetTickerTypesTests
{
    private readonly Mock<IPolygonReferenceApi> _mockApi;
    private readonly ReferenceDataService _service;

    /// <summary>
    /// Initializes a new instance of the ReferenceDataService_GetTickerTypesTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public ReferenceDataService_GetTickerTypesTests()
    {
        _mockApi = new Mock<IPolygonReferenceApi>();
        _service = new ReferenceDataService(_mockApi.Object);
    }

    /// <summary>
    /// Tests that GetTickerTypesAsync calls the API and returns the ticker types.
    /// </summary>
    [Fact]
    public async Task GetTickerTypesAsync_CallsApi_ReturnsTickerTypes()
    {
        // Arrange
        var expectedResponse = new TickerTypesResponse
        {
            Results = new List<TickerType>
            {
                new TickerType
                {
                    Code = "CS",
                    Description = "Common Stock",
                    AssetClass = "stocks",
                    Locale = "us"
                },
                new TickerType
                {
                    Code = "ETF",
                    Description = "Exchange Traded Fund",
                    AssetClass = "stocks",
                    Locale = "us"
                }
            },
            Count = 2,
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetTickerTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Count, result.Count);
        Assert.Equal(expectedResponse.Status, result.Status);
        Assert.Equal(expectedResponse.RequestId, result.RequestId);
        Assert.NotNull(result.Results);
        Assert.Equal(2, result.Results.Count);

        Assert.Equal("CS", result.Results[0].Code);
        Assert.Equal("Common Stock", result.Results[0].Description);
        Assert.Equal("stocks", result.Results[0].AssetClass);
        Assert.Equal("us", result.Results[0].Locale);

        Assert.Equal("ETF", result.Results[1].Code);
        Assert.Equal("Exchange Traded Fund", result.Results[1].Description);
        Assert.Equal("stocks", result.Results[1].AssetClass);
        Assert.Equal("us", result.Results[1].Locale);

        _mockApi.Verify(x => x.GetTickerTypesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetTickerTypesAsync passes the cancellation token to the API.
    /// </summary>
    [Fact]
    public async Task GetTickerTypesAsync_PassesCancellationToken()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var expectedResponse = new TickerTypesResponse
        {
            Results = new List<TickerType>(),
            Status = "OK",
            RequestId = "test-request-id",
            Count = 0
        };

        _mockApi.Setup(x => x.GetTickerTypesAsync(cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.GetTickerTypesAsync(cancellationToken);

        // Assert
        _mockApi.Verify(x => x.GetTickerTypesAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// Tests that GetTickerTypesAsync handles empty results correctly.
    /// </summary>
    [Fact]
    public async Task GetTickerTypesAsync_WithEmptyResults_ReturnsEmptyList()
    {
        // Arrange
        var expectedResponse = new TickerTypesResponse
        {
            Results = new List<TickerType>(),
            Count = 0,
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetTickerTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
        Assert.Equal("OK", result.Status);
        Assert.Empty(result.Results);
    }

    /// <summary>
    /// Tests that GetTickerTypesAsync handles null response from API.
    /// </summary>
    [Fact]
    public async Task GetTickerTypesAsync_WhenApiReturnsNull_ReturnsNull()
    {
        // Arrange
        _mockApi.Setup(x => x.GetTickerTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((TickerTypesResponse)null!);

        // Act
        var result = await _service.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetTickerTypesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetTickerTypesAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetTickerTypesAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetTickerTypesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetTickerTypesAsync(TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetTickerTypesAsync handles all ticker types from the real API response.
    /// </summary>
    [Fact]
    public async Task GetTickerTypesAsync_WithAllTickerTypes_ReturnsCompleteData()
    {
        // Arrange
        var expectedResponse = new TickerTypesResponse
        {
            Results = new List<TickerType>
            {
                new TickerType { Code = "CS", Description = "Common Stock", AssetClass = "stocks", Locale = "us" },
                new TickerType { Code = "PFD", Description = "Preferred Stock", AssetClass = "stocks", Locale = "us" },
                new TickerType { Code = "WARRANT", Description = "Warrant", AssetClass = "stocks", Locale = "us" },
                new TickerType { Code = "RIGHT", Description = "Rights", AssetClass = "stocks", Locale = "us" },
                new TickerType { Code = "BOND", Description = "Corporate Bond", AssetClass = "stocks", Locale = "us" },
                new TickerType { Code = "ETF", Description = "Exchange Traded Fund", AssetClass = "stocks", Locale = "us" },
                new TickerType { Code = "IX", Description = "Index", AssetClass = "indices", Locale = "us" }
            },
            Count = 7,
            Status = "OK",
            RequestId = "test-request-id"
        };

        _mockApi.Setup(x => x.GetTickerTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Count);
        Assert.Equal("OK", result.Status);
        Assert.Equal(7, result.Results.Count);

        // Verify specific ticker types
        var commonStock = result.Results.First(t => t.Code == "CS");
        Assert.Equal("Common Stock", commonStock.Description);
        Assert.Equal("stocks", commonStock.AssetClass);
        Assert.Equal("us", commonStock.Locale);

        var etf = result.Results.First(t => t.Code == "ETF");
        Assert.Equal("Exchange Traded Fund", etf.Description);
        Assert.Equal("stocks", etf.AssetClass);
        Assert.Equal("us", etf.Locale);

        var index = result.Results.First(t => t.Code == "IX");
        Assert.Equal("Index", index.Description);
        Assert.Equal("indices", index.AssetClass);
        Assert.Equal("us", index.Locale);
    }
}
