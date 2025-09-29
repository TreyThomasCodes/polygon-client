using Moq;
using TTC.Polygon.RestClient.Api;
using TTC.Polygon.RestClient.Services;
using TTC.Polygon.Models.Reference;

namespace TTC.Polygon.RestClient.Tests.Services;

/// <summary>
/// Unit tests for the ReferenceDataService class.
/// Tests the service's ability to retrieve market status and other reference data from Polygon.io API.
/// </summary>
public class ReferenceDataServiceTests
{
    private readonly Mock<IPolygonReferenceApi> _mockApi;
    private readonly ReferenceDataService _service;

    /// <summary>
    /// Initializes a new instance of the ReferenceDataServiceTests class.
    /// Sets up the mock API and service instance for testing.
    /// </summary>
    public ReferenceDataServiceTests()
    {
        _mockApi = new Mock<IPolygonReferenceApi>();
        _service = new ReferenceDataService(_mockApi.Object);
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when api parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenApiIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ReferenceDataService(null!));
    }

    /// <summary>
    /// Tests that GetMarketStatusAsync calls the API and returns the market status.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_CallsApi_ReturnsMarketStatus()
    {
        // Arrange
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
        var result = await _service.GetMarketStatusAsync(TestContext.Current.CancellationToken);

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
        var cancellationToken = new CancellationToken();
        var expectedMarketStatus = new MarketStatus { Market = "closed" };

        _mockApi.Setup(x => x.GetMarketStatusAsync(cancellationToken))
            .ReturnsAsync(expectedMarketStatus);

        // Act
        await _service.GetMarketStatusAsync(cancellationToken);

        // Assert
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
        var result = await _service.GetMarketStatusAsync(TestContext.Current.CancellationToken);

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
        _mockApi.Setup(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((MarketStatus)null!);

        // Act
        var result = await _service.GetMarketStatusAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
        _mockApi.Verify(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetMarketStatusAsync propagates exceptions from the API.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_WhenApiThrowsException_PropagatesException()
    {
        // Arrange
        var expectedException = new HttpRequestException("Network error");
        _mockApi.Setup(x => x.GetMarketStatusAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(
            () => _service.GetMarketStatusAsync(TestContext.Current.CancellationToken));
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    /// <summary>
    /// Tests that GetMarketStatusAsync handles complex market status with all nested objects.
    /// </summary>
    [Fact]
    public async Task GetMarketStatusAsync_WithComplexMarketStatus_ReturnsCompleteData()
    {
        // Arrange
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
        var result = await _service.GetMarketStatusAsync(TestContext.Current.CancellationToken);

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