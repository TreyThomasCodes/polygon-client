// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.RestClient.Exceptions;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

namespace TreyThomasCodes.Polygon.RestClient.Services;

/// <summary>
/// Implementation of the reference data service for accessing Polygon.io reference data and market status information.
/// Provides methods to retrieve ticker information, market metadata, trading status, and other reference information.
/// </summary>
internal class ReferenceDataService : IReferenceDataService
{
    private readonly IPolygonReferenceApi _api;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReferenceDataService> _logger;

    /// <summary>
    /// Initializes a new instance of the ReferenceDataService with the specified API client, service provider, and logger.
    /// </summary>
    /// <param name="api">The Polygon.io reference API client used for making HTTP requests.</param>
    /// <param name="serviceProvider">The service provider used to retrieve validators for request objects.</param>
    /// <param name="logger">The logger used for logging errors and diagnostic information.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api, serviceProvider, or logger parameter is null.</exception>
    public ReferenceDataService(IPolygonReferenceApi api, IServiceProvider serviceProvider, ILogger<ReferenceDataService> logger)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<StockTicker>>> GetTickersAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetTickersRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetTickersAsync(
                request.Ticker,
                request.TickerGt,
                request.TickerGte,
                request.TickerLt,
                request.TickerLte,
                request.Type,
                request.Market,
                request.Exchange,
                request.Cusip,
                request.Cik,
                request.Date,
                request.Search,
                request.Active,
                request.Sort,
                request.Order,
                request.Limit,
                cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method}", nameof(GetTickersAsync));
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method}: {StatusCode}", nameof(GetTickersAsync), ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method}", nameof(GetTickersAsync));
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method}", nameof(GetTickersAsync));
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method}", nameof(GetTickersAsync));
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<StockTicker>> GetTickerDetailsAsync(
        GetTickerDetailsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetTickerDetailsRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetTickerDetailsAsync(request.Ticker, request.Date, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} with ticker {Ticker}", nameof(GetTickerDetailsAsync), request.Ticker);
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method} for ticker {Ticker}: {StatusCode}", nameof(GetTickerDetailsAsync), request.Ticker, ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for ticker {Ticker}", nameof(GetTickerDetailsAsync), request.Ticker);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method} for ticker {Ticker}", nameof(GetTickerDetailsAsync), request.Ticker);
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method} for ticker {Ticker}", nameof(GetTickerDetailsAsync), request.Ticker);
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public Task<PolygonResponse<StockTicker>> GetTickerDetailsAsync(
        string ticker,
        CancellationToken cancellationToken = default)
    {
        var request = new GetTickerDetailsRequest { Ticker = ticker };
        return GetTickerDetailsAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<MarketStatus> GetMarketStatusAsync(
        GetMarketStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetMarketStatusRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetMarketStatusAsync(cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method}", nameof(GetMarketStatusAsync));
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method}: {StatusCode}", nameof(GetMarketStatusAsync), ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method}", nameof(GetMarketStatusAsync));
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method}", nameof(GetMarketStatusAsync));
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method}", nameof(GetMarketStatusAsync));
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<TickerTypesResponse> GetTickerTypesAsync(
        GetTickerTypesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetTickerTypesRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetTickerTypesAsync(cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method}", nameof(GetTickerTypesAsync));
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method}: {StatusCode}", nameof(GetTickerTypesAsync), ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method}", nameof(GetTickerTypesAsync));
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method}", nameof(GetTickerTypesAsync));
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method}", nameof(GetTickerTypesAsync));
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<ConditionCode>>> GetConditionCodesAsync(
        GetConditionCodesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetConditionCodesRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetConditionCodesAsync(
                request.AssetClass,
                request.DataType,
                request.Id,
                request.SipMapping,
                request.Order,
                request.Limit,
                request.Sort,
                cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method}", nameof(GetConditionCodesAsync));
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method}: {StatusCode}", nameof(GetConditionCodesAsync), ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method}", nameof(GetConditionCodesAsync));
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method}", nameof(GetConditionCodesAsync));
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method}", nameof(GetConditionCodesAsync));
            throw PolygonHttpException.FromTimeout(ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PolygonValidationException">Thrown when the request parameters fail validation.</exception>
    /// <exception cref="PolygonApiException">Thrown when the Polygon.io API returns an error response.</exception>
    /// <exception cref="PolygonHttpException">Thrown when a network or HTTP transport error occurs.</exception>
    public async Task<PolygonResponse<List<Exchange>>> GetExchangesAsync(
        GetExchangesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<GetExchangesRequest>>();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            return await _api.GetExchangesAsync(request.AssetClass, request.Locale, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method}", nameof(GetExchangesAsync));
            throw new PolygonValidationException(ex);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error calling {Method}: {StatusCode}", nameof(GetExchangesAsync), ex.StatusCode);
            throw new PolygonApiException(ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method}", nameof(GetExchangesAsync));
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Method}", nameof(GetExchangesAsync));
            throw PolygonHttpException.FromHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling {Method}", nameof(GetExchangesAsync));
            throw PolygonHttpException.FromTimeout(ex);
        }
    }
}
