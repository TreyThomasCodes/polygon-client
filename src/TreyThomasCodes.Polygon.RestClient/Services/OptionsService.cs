// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Exceptions;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.RestClient.Services;

/// <summary>
/// Implementation of the options service for accessing Polygon.io options market data.
/// Provides methods to retrieve options contract information, trades, quotes, snapshots, and OHLC aggregates.
/// This service acts as a wrapper around the Polygon.io Options API, providing a convenient interface for accessing options data.
/// </summary>
public class OptionsService : IOptionsService
{
    private readonly IPolygonOptionsApi _api;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OptionsService> _logger;

    /// <summary>
    /// Initializes a new instance of the OptionsService with the specified API client, service provider, and logger.
    /// </summary>
    /// <param name="api">The Polygon.io options API client used for making HTTP requests.</param>
    /// <param name="serviceProvider">The service provider used to retrieve validators for request objects.</param>
    /// <param name="logger">The logger used for logging errors and diagnostic information.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api, serviceProvider, or logger parameter is null.</exception>
    public OptionsService(IPolygonOptionsApi api, IServiceProvider serviceProvider, ILogger<OptionsService> logger)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<OptionsContract>> GetContractDetailsAsync(
        GetContractDetailsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetContractDetailsRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetContractDetailsAsync(request.OptionsTicker, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetContractDetailsAsync), request.OptionsTicker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetContractDetailsAsync), request.OptionsTicker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetContractDetailsAsync), request.OptionsTicker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetContractDetailsAsync), request.OptionsTicker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<OptionSnapshot>> GetSnapshotAsync(
        GetSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetSnapshotRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetSnapshotAsync(request.UnderlyingAsset, request.OptionContract, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with underlying {Underlying}", nameof(GetSnapshotAsync), request.UnderlyingAsset);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for underlying {Underlying}: {StatusCode}", nameof(GetSnapshotAsync), request.UnderlyingAsset, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for underlying {Underlying}", nameof(GetSnapshotAsync), request.UnderlyingAsset);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for underlying {Underlying}", nameof(GetSnapshotAsync), request.UnderlyingAsset);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<OptionSnapshot>>> GetChainSnapshotAsync(
        GetChainSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetChainSnapshotRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetChainSnapshotAsync(
                request.UnderlyingAsset,
                request.StrikePrice,
                request.ContractType,
                request.ExpirationDateGte,
                request.ExpirationDateLte,
                request.Limit,
                request.Order,
                request.Sort,
                request.Cursor,
                cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with underlying {Underlying}", nameof(GetChainSnapshotAsync), request.UnderlyingAsset);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for underlying {Underlying}: {StatusCode}", nameof(GetChainSnapshotAsync), request.UnderlyingAsset, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for underlying {Underlying}", nameof(GetChainSnapshotAsync), request.UnderlyingAsset);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for underlying {Underlying}", nameof(GetChainSnapshotAsync), request.UnderlyingAsset);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<OptionTrade>> GetLastTradeAsync(
        GetLastTradeRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetLastTradeRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetLastTradeAsync(request.OptionsTicker, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetLastTradeAsync), request.OptionsTicker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetLastTradeAsync), request.OptionsTicker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetLastTradeAsync), request.OptionsTicker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetLastTradeAsync), request.OptionsTicker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<OptionQuote>>> GetQuotesAsync(
        GetQuotesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetQuotesRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetQuotesAsync(
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
                cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetQuotesAsync), request.OptionsTicker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetQuotesAsync), request.OptionsTicker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetQuotesAsync), request.OptionsTicker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetQuotesAsync), request.OptionsTicker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<OptionTradeV3>>> GetTradesAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetTradesRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetTradesAsync(
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
                cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetTradesAsync), request.OptionsTicker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetTradesAsync), request.OptionsTicker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetTradesAsync), request.OptionsTicker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetTradesAsync), request.OptionsTicker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<OptionBar>>> GetBarsAsync(
        GetBarsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetBarsRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetBarsAsync(
                request.OptionsTicker,
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
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetBarsAsync), request.OptionsTicker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetBarsAsync), request.OptionsTicker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetBarsAsync), request.OptionsTicker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetBarsAsync), request.OptionsTicker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<OptionDailyOpenClose> GetDailyOpenCloseAsync(
        GetDailyOpenCloseRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetDailyOpenCloseRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetDailyOpenCloseAsync(request.OptionsTicker, request.Date, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetDailyOpenCloseAsync), request.OptionsTicker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetDailyOpenCloseAsync), request.OptionsTicker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetDailyOpenCloseAsync), request.OptionsTicker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetDailyOpenCloseAsync), request.OptionsTicker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<OptionBar>>> GetPreviousDayBarAsync(
        GetPreviousDayBarRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetPreviousDayBarRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetPreviousDayBarAsync(request.OptionsTicker, request.Adjusted, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetPreviousDayBarAsync), request.OptionsTicker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetPreviousDayBarAsync), request.OptionsTicker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetPreviousDayBarAsync), request.OptionsTicker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetPreviousDayBarAsync), request.OptionsTicker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }
}
