// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Exceptions;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Services;

/// <summary>
/// Implementation of the stocks service for accessing Polygon.io stocks market data.
/// Provides methods to retrieve real-time and historical stock trades, quotes, snapshots, and OHLC aggregates.
/// </summary>
public class StocksService : IStocksService
{
    private readonly IPolygonStocksApi _api;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StocksService> _logger;

    /// <summary>
    /// Initializes a new instance of the StocksService with the specified API client, service provider, and logger.
    /// </summary>
    /// <param name="api">The Polygon.io stocks API client used for making HTTP requests.</param>
    /// <param name="serviceProvider">The service provider used to retrieve validators for request objects.</param>
    /// <param name="logger">The logger used for logging errors and diagnostic information.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api, serviceProvider, or logger parameter is null.</exception>
    public StocksService(IPolygonStocksApi api, IServiceProvider serviceProvider, ILogger<StocksService> logger)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<StockBar>>> GetBarsAsync(
        GetBarsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetBarsRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetBarsAsync(
                request.Ticker,
                request.Multiplier,
                request.Timespan,
                request.From,
                request.To,
                request.Adjusted,
                request.Sort,
                request.Limit,
                cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetBarsAsync), request.Ticker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetBarsAsync), request.Ticker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetBarsAsync), request.Ticker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetBarsAsync), request.Ticker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<StockBar>>> GetPreviousCloseAsync(
        GetPreviousCloseRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetPreviousCloseRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetPreviousCloseAsync(request.Ticker, request.Adjusted, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetPreviousCloseAsync), request.Ticker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetPreviousCloseAsync), request.Ticker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetPreviousCloseAsync), request.Ticker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetPreviousCloseAsync), request.Ticker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<StockBar>>> GetGroupedDailyAsync(
        GetGroupedDailyRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetGroupedDailyRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetGroupedDailyAsync(request.Date, request.Adjusted, request.IncludeOtc, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with date {Date}", nameof(GetGroupedDailyAsync), request.Date);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for date {Date}: {StatusCode}", nameof(GetGroupedDailyAsync), request.Date, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for date {Date}", nameof(GetGroupedDailyAsync), request.Date);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for date {Date}", nameof(GetGroupedDailyAsync), request.Date);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<StockBar>>> GetDailyOpenCloseAsync(
        GetDailyOpenCloseRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetDailyOpenCloseRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetDailyOpenCloseAsync(request.Ticker, request.Date, request.Adjusted, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetDailyOpenCloseAsync), request.Ticker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetDailyOpenCloseAsync), request.Ticker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetDailyOpenCloseAsync), request.Ticker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetDailyOpenCloseAsync), request.Ticker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<StockTrade>>> GetTradesAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetTradesRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetTradesAsync(
                request.Ticker,
                request.Timestamp,
                request.TimestampGte,
                request.TimestampGt,
                request.TimestampLte,
                request.TimestampLt,
                request.Limit,
                request.Sort,
                cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetTradesAsync), request.Ticker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetTradesAsync), request.Ticker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetTradesAsync), request.Ticker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetTradesAsync), request.Ticker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<StockQuote>>> GetQuotesAsync(
        GetQuotesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetQuotesRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetQuotesAsync(
                request.Ticker,
                request.Timestamp,
                request.TimestampGte,
                request.TimestampGt,
                request.TimestampLte,
                request.TimestampLt,
                request.Limit,
                request.Sort,
                cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetQuotesAsync), request.Ticker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetQuotesAsync), request.Ticker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetQuotesAsync), request.Ticker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetQuotesAsync), request.Ticker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<StockSnapshot>>> GetMarketSnapshotAsync(
        GetMarketSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetMarketSnapshotRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetMarketSnapshotAsync(request.IncludeOtc, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method}", nameof(GetMarketSnapshotAsync));
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method}: {StatusCode}", nameof(GetMarketSnapshotAsync), ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method}", nameof(GetMarketSnapshotAsync));
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method}", nameof(GetMarketSnapshotAsync));
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<StockSnapshotResponse> GetSnapshotAsync(
        GetSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetSnapshotRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetSnapshotAsync(request.Ticker, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetSnapshotAsync), request.Ticker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetSnapshotAsync), request.Ticker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetSnapshotAsync), request.Ticker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetSnapshotAsync), request.Ticker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<StockTrade>> GetLastTradeAsync(
        GetLastTradeRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetLastTradeRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetLastTradeAsync(request.Ticker, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetLastTradeAsync), request.Ticker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetLastTradeAsync), request.Ticker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetLastTradeAsync), request.Ticker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetLastTradeAsync), request.Ticker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<LastQuoteResult>> GetLastQuoteAsync(
        GetLastQuoteRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetLastQuoteRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetLastQuoteAsync(request.Ticker, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetLastQuoteAsync), request.Ticker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetLastQuoteAsync), request.Ticker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetLastQuoteAsync), request.Ticker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetLastQuoteAsync), request.Ticker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }
}
