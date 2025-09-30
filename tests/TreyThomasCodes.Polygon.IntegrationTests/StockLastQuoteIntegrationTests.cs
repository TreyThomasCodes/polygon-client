// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching the last stock quote (NBBO) data.
/// These tests require a valid Polygon API key to be configured in user secrets.
/// </summary>
public class StockLastQuoteIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the StockLastQuoteIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public StockLastQuoteIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<StockLastQuoteIntegrationTests>();
        var apiKey = builder.Configuration["Polygon:ApiKey"];

        // Skip all tests in this class if no API key is configured
        Assert.SkipUnless(!string.IsNullOrEmpty(apiKey), "Polygon API key not configured in user secrets. Use: dotnet user-secrets set \"Polygon:ApiKey\" \"your-api-key-here\"");

        _apiKey = apiKey!; // Safe to use ! since we skip if null or empty

        builder.Services.AddPolygonClient(options =>
        {
            options.ApiKey = _apiKey;
        });

        _host = builder.Build();
        _polygonClient = _host.Services.GetRequiredService<IPolygonClient>();
    }

    /// <summary>
    /// Tests fetching the last quote for AAPL and verifies the response structure and data validity.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_ForAAPL_ShouldReturnValidLastQuoteData()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);

        // Verify quote data is present
        Assert.NotNull(response.Results);
        Assert.Equal(ticker, response.Results.Ticker);

        // Verify bid and ask prices are valid
        Assert.NotNull(response.Results.BidPrice);
        Assert.True(response.Results.BidPrice > 0, "Bid price should be greater than 0");
        Assert.NotNull(response.Results.AskPrice);
        Assert.True(response.Results.AskPrice > 0, "Ask price should be greater than 0");

        // Verify bid and ask sizes are valid
        Assert.NotNull(response.Results.BidSize);
        Assert.True(response.Results.BidSize > 0, "Bid size should be greater than 0");
        Assert.NotNull(response.Results.AskSize);
        Assert.True(response.Results.AskSize > 0, "Ask size should be greater than 0");

        // Verify timestamp is present and recent (within last 7 days)
        Assert.NotNull(response.Results.Timestamp);
        var quoteTime = Instant.FromUnixTimeTicks(response.Results.Timestamp.Value / 100);
        var currentTime = SystemClock.Instance.GetCurrentInstant();
        var timeDifference = currentTime - quoteTime;
        Assert.True(timeDifference.TotalDays <= 7, $"Quote timestamp should be recent, but was {timeDifference.TotalDays} days old");

        // Verify exchanges are present
        Assert.NotNull(response.Results.BidExchange);
        Assert.True(response.Results.BidExchange >= 0, "Bid exchange code should be non-negative");
        Assert.NotNull(response.Results.AskExchange);
        Assert.True(response.Results.AskExchange >= 0, "Ask exchange code should be non-negative");
    }

    /// <summary>
    /// Tests fetching the last quote for multiple popular stock tickers.
    /// </summary>
    [Theory]
    [InlineData("AAPL")]
    [InlineData("MSFT")]
    [InlineData("GOOGL")]
    [InlineData("TSLA")]
    public async Task GetLastQuoteAsync_ForPopularTickers_ShouldReturnValidData(string ticker)
    {
        // Arrange
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response.Status);
        Assert.NotNull(response.RequestId);
        Assert.NotEmpty(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.Equal(ticker, response.Results.Ticker);

        // Basic validation that we got meaningful data
        Assert.NotNull(response.Results.BidPrice);
        Assert.True(response.Results.BidPrice > 0, "Bid price should be greater than 0");
        Assert.NotNull(response.Results.AskPrice);
        Assert.True(response.Results.AskPrice > 0, "Ask price should be greater than 0");
        Assert.NotNull(response.Results.BidSize);
        Assert.True(response.Results.BidSize > 0, "Bid size should be greater than 0");
        Assert.NotNull(response.Results.AskSize);
        Assert.True(response.Results.AskSize > 0, "Ask size should be greater than 0");
        Assert.NotNull(response.Results.Timestamp);
        Assert.True(response.Results.Timestamp > 0, "Timestamp should be positive");
    }

    /// <summary>
    /// Tests the data types and structure of the last quote response.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<Models.Common.PolygonResponse<Models.Stocks.LastQuoteResult>>(response);

        // Verify response structure
        Assert.IsType<string>(response.Status);
        Assert.IsType<string>(response.RequestId);

        Assert.NotNull(response.Results);
        Assert.IsType<Models.Stocks.LastQuoteResult>(response.Results);
        Assert.IsType<string>(response.Results.Ticker);
        Assert.IsType<decimal>(response.Results.BidPrice);
        Assert.IsType<decimal>(response.Results.AskPrice);
        Assert.IsType<long>(response.Results.BidSize);
        Assert.IsType<long>(response.Results.AskSize);
        Assert.IsType<long>(response.Results.Timestamp);
        Assert.IsType<int>(response.Results.BidExchange);
        Assert.IsType<int>(response.Results.AskExchange);

        // Verify optional fields if present
        if (response.Results.Indicators != null)
            Assert.IsType<List<int>>(response.Results.Indicators);

        if (response.Results.Sequence.HasValue)
            Assert.IsType<long>(response.Results.Sequence.Value);

        if (response.Results.ParticipantTimestamp.HasValue)
            Assert.IsType<long>(response.Results.ParticipantTimestamp.Value);

        if (response.Results.Tape.HasValue)
            Assert.IsType<int>(response.Results.Tape.Value);
    }


    /// <summary>
    /// Tests behavior when requesting the last quote for an invalid ticker symbol.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_ForInvalidTicker_ShouldThrowApiException()
    {
        // Arrange
        var invalidTicker = "INVALIDTICKER123";
        var stocksService = _polygonClient.Stocks;

        // Act & Assert
        // The Polygon API returns a 404 for invalid ticker symbols
        // This is expected behavior and should throw a Refit.ApiException
        var exception = await Assert.ThrowsAsync<Refit.ApiException>(
            () => stocksService.GetLastQuoteAsync(invalidTicker, TestContext.Current.CancellationToken));

        // Verify the exception details
        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("404", exception.Message);
    }

    /// <summary>
    /// Tests that tape designation is valid when present.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_Tape_ShouldBeValidIfPresent()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        // Only verify if Tape is present
        if (response.Results.Tape.HasValue)
        {
            // Tape should be 1, 2, or 3 (representing Tape A, B, or C)
            Assert.InRange(response.Results.Tape.Value, 1, 3);
        }
    }

    /// <summary>
    /// Tests that indicators are present and valid when returned.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_Indicators_ShouldBeValidIfPresent()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        // Only verify if Indicators are present
        if (response.Results.Indicators != null)
        {
            Assert.NotEmpty(response.Results.Indicators);

            // Verify all indicator codes are non-negative
            foreach (var indicator in response.Results.Indicators)
            {
                Assert.True(indicator >= 0, $"Indicator code {indicator} should be non-negative");
            }
        }
    }

    /// <summary>
    /// Tests that sequence number is valid when present.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_Sequence_ShouldBeValidIfPresent()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        // Only verify if Sequence is present
        if (response.Results.Sequence.HasValue)
        {
            Assert.True(response.Results.Sequence.Value >= 0, "Sequence number should be non-negative");
        }
    }

    /// <summary>
    /// Tests that participant timestamp is valid and reasonable when present.
    /// </summary>
    [Fact]
    public async Task GetLastQuoteAsync_ParticipantTimestamp_ShouldBeValidIfPresent()
    {
        // Arrange
        var ticker = "AAPL";
        var stocksService = _polygonClient.Stocks;

        // Act
        var response = await stocksService.GetLastQuoteAsync(ticker, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Results);

        // Only verify if ParticipantTimestamp is present
        if (response.Results.ParticipantTimestamp.HasValue)
        {
            Assert.True(response.Results.ParticipantTimestamp.Value > 0, "Participant timestamp should be positive");

            // Verify the participant timestamp is reasonable (not in the future, not too far in the past)
            var participantTime = Instant.FromUnixTimeTicks(response.Results.ParticipantTimestamp.Value / 100);
            var currentTime = SystemClock.Instance.GetCurrentInstant();
            Assert.True(participantTime <= currentTime, "Participant timestamp should not be in the future");

            var timeDifference = currentTime - participantTime;
            Assert.True(timeDifference.TotalDays <= 7, "Participant timestamp should be recent (within 7 days)");
        }
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _host.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
