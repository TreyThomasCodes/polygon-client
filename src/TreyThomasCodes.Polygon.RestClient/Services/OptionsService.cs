// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;
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
internal class OptionsService : IOptionsService
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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for ticker {Ticker}", nameof(GetContractDetailsAsync), request.OptionsTicker);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for underlying {Underlying}", nameof(GetSnapshotAsync), request.UnderlyingAsset);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for underlying {Underlying}", nameof(GetChainSnapshotAsync), request.UnderlyingAsset);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for ticker {Ticker}", nameof(GetLastTradeAsync), request.OptionsTicker);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for ticker {Ticker}", nameof(GetQuotesAsync), request.OptionsTicker);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for ticker {Ticker}", nameof(GetTradesAsync), request.OptionsTicker);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for ticker {Ticker}", nameof(GetBarsAsync), request.OptionsTicker);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for ticker {Ticker}", nameof(GetDailyOpenCloseAsync), request.OptionsTicker);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error calling {Method} for ticker {Ticker}", nameof(GetPreviousDayBarAsync), request.OptionsTicker);
            throw new PolygonException("Failed to deserialize API response. The data format may be invalid.", ex);
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

    // Component-based convenience methods

    /// <inheritdoc />
    public Task<PolygonResponse<OptionsContract>> GetContractByComponentsAsync(
        string underlying,
        DateOnly expirationDate,
        OptionType type,
        decimal strike,
        CancellationToken cancellationToken = default)
    {
        string ticker = OptionsTicker.Create(underlying, expirationDate, type, strike);
        var request = new GetContractDetailsRequest { OptionsTicker = ticker };
        return GetContractDetailsAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<OptionSnapshot>> GetSnapshotByComponentsAsync(
        string underlying,
        DateOnly expirationDate,
        OptionType type,
        decimal strike,
        CancellationToken cancellationToken = default)
    {
        var ticker = new OptionsTicker(underlying, expirationDate, type, strike);
        var request = new GetSnapshotRequest
        {
            UnderlyingAsset = underlying,
            OptionContract = ticker.ToString()
        };
        return GetSnapshotAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<OptionSnapshot>>> GetChainSnapshotByComponentsAsync(
        string underlyingAsset,
        decimal? strikePrice = null,
        OptionType? type = null,
        DateOnly? expirationDateGte = null,
        DateOnly? expirationDateLte = null,
        int? limit = null,
        string? order = null,
        string? sort = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        string? contractType = type switch
        {
            OptionType.Call => "call",
            OptionType.Put => "put",
            null => null,
            _ => null
        };

        var request = new GetChainSnapshotRequest
        {
            UnderlyingAsset = underlyingAsset,
            StrikePrice = strikePrice,
            ContractType = contractType,
            ExpirationDateGte = expirationDateGte?.ToString("yyyy-MM-dd"),
            ExpirationDateLte = expirationDateLte?.ToString("yyyy-MM-dd"),
            Limit = limit,
            Order = order,
            Sort = sort,
            Cursor = cursor
        };
        return GetChainSnapshotAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<OptionTrade>> GetLastTradeByComponentsAsync(
        string underlying,
        DateOnly expirationDate,
        OptionType type,
        decimal strike,
        CancellationToken cancellationToken = default)
    {
        string ticker = OptionsTicker.Create(underlying, expirationDate, type, strike);
        var request = new GetLastTradeRequest { OptionsTicker = ticker };
        return GetLastTradeAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<OptionBar>>> GetBarsByComponentsAsync(
        string underlying,
        DateOnly expirationDate,
        OptionType type,
        decimal strike,
        int multiplier,
        AggregateInterval timespan,
        string from,
        string to,
        bool? adjusted = null,
        SortOrder? sort = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        string ticker = OptionsTicker.Create(underlying, expirationDate, type, strike);
        var request = new GetBarsRequest
        {
            OptionsTicker = ticker,
            Multiplier = multiplier,
            Timespan = timespan,
            From = from,
            To = to,
            Adjusted = adjusted,
            Sort = sort,
            Limit = limit
        };
        return GetBarsAsync(request, cancellationToken);
    }

    // Discovery helper methods

    /// <inheritdoc />
    public async Task<List<decimal>> GetAvailableStrikesAsync(
        string underlying,
        OptionType? type = null,
        string? expirationDateGte = null,
        string? expirationDateLte = null,
        CancellationToken cancellationToken = default)
    {
        string? contractType = type switch
        {
            OptionType.Call => "call",
            OptionType.Put => "put",
            null => null,
            _ => null
        };

        var request = new GetChainSnapshotRequest
        {
            UnderlyingAsset = underlying,
            ContractType = contractType,
            ExpirationDateGte = expirationDateGte,
            ExpirationDateLte = expirationDateLte,
            Limit = 250, // Max batch size
            Sort = "strike_price",
            Order = "asc"
        };

        var response = await GetChainSnapshotAsync(request, cancellationToken);

        if (response.Results == null || response.Results.Count == 0)
            return new List<decimal>();

        // Extract unique strike prices and sort them
        var strikes = response.Results
            .Select(snapshot => snapshot.Details?.StrikePrice)
            .Where(strike => strike.HasValue)
            .Select(strike => strike!.Value)
            .Distinct()
            .OrderBy(strike => strike)
            .ToList();

        return strikes;
    }

    /// <inheritdoc />
    public async Task<List<DateOnly>> GetExpirationDatesAsync(
        string underlying,
        OptionType? type = null,
        decimal? strikePrice = null,
        CancellationToken cancellationToken = default)
    {
        string? contractType = type switch
        {
            OptionType.Call => "call",
            OptionType.Put => "put",
            null => null,
            _ => null
        };

        var request = new GetChainSnapshotRequest
        {
            UnderlyingAsset = underlying,
            StrikePrice = strikePrice,
            ContractType = contractType,
            Limit = 250, // Max batch size
            Sort = "expiration_date",
            Order = "asc"
        };

        var response = await GetChainSnapshotAsync(request, cancellationToken);

        if (response.Results == null || response.Results.Count == 0)
            return new List<DateOnly>();

        // Extract unique expiration dates and sort them
        var dates = new List<DateOnly>();
        foreach (var snapshot in response.Results)
        {
            var dateString = snapshot.Details?.ExpirationDate;
            if (string.IsNullOrEmpty(dateString))
                continue;

            if (DateOnly.TryParse(dateString, out var parsedDate))
            {
                dates.Add(parsedDate);
            }
            else
            {
                // If the API returns an invalid date format, throw a PolygonException
                throw new PolygonException($"Invalid date format received from Polygon.io API: '{dateString}'. Expected a valid date string.");
            }
        }

        return dates.Distinct().OrderBy(date => date).ToList();
    }

    // OptionsTicker-based overload methods

    /// <inheritdoc />
    public Task<PolygonResponse<OptionsContract>> GetContractDetailsAsync(
        OptionsTicker ticker,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        var request = new GetContractDetailsRequest { OptionsTicker = ticker.ToString() };
        return GetContractDetailsAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<OptionSnapshot>> GetSnapshotAsync(
        OptionsTicker ticker,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        var request = new GetSnapshotRequest
        {
            UnderlyingAsset = ticker.Underlying,
            OptionContract = ticker.ToString()
        };
        return GetSnapshotAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<OptionTrade>> GetLastTradeAsync(
        OptionsTicker ticker,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        var request = new GetLastTradeRequest { OptionsTicker = ticker.ToString() };
        return GetLastTradeAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<OptionQuote>>> GetQuotesAsync(
        OptionsTicker ticker,
        string? timestamp = null,
        string? timestampLt = null,
        string? timestampLte = null,
        string? timestampGt = null,
        string? timestampGte = null,
        string? order = null,
        int? limit = null,
        string? sort = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        var request = new GetQuotesRequest
        {
            OptionsTicker = ticker.ToString(),
            Timestamp = timestamp,
            TimestampLt = timestampLt,
            TimestampLte = timestampLte,
            TimestampGt = timestampGt,
            TimestampGte = timestampGte,
            Order = order,
            Limit = limit,
            Sort = sort,
            Cursor = cursor
        };
        return GetQuotesAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<OptionTradeV3>>> GetTradesAsync(
        OptionsTicker ticker,
        string? timestamp = null,
        string? timestampLt = null,
        string? timestampLte = null,
        string? timestampGt = null,
        string? timestampGte = null,
        string? order = null,
        int? limit = null,
        string? sort = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        var request = new GetTradesRequest
        {
            OptionsTicker = ticker.ToString(),
            Timestamp = timestamp,
            TimestampLt = timestampLt,
            TimestampLte = timestampLte,
            TimestampGt = timestampGt,
            TimestampGte = timestampGte,
            Order = order,
            Limit = limit,
            Sort = sort,
            Cursor = cursor
        };
        return GetTradesAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<OptionBar>>> GetBarsAsync(
        OptionsTicker ticker,
        int multiplier,
        AggregateInterval timespan,
        string from,
        string to,
        bool? adjusted = null,
        SortOrder? sort = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        var request = new GetBarsRequest
        {
            OptionsTicker = ticker.ToString(),
            Multiplier = multiplier,
            Timespan = timespan,
            From = from,
            To = to,
            Adjusted = adjusted,
            Sort = sort,
            Limit = limit
        };
        return GetBarsAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<OptionDailyOpenClose> GetDailyOpenCloseAsync(
        OptionsTicker ticker,
        string date,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        var request = new GetDailyOpenCloseRequest
        {
            OptionsTicker = ticker.ToString(),
            Date = date
        };
        return GetDailyOpenCloseAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<OptionBar>>> GetPreviousDayBarAsync(
        OptionsTicker ticker,
        bool? adjusted = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = ticker.ToString(),
            Adjusted = adjusted
        };
        return GetPreviousDayBarAsync(request, cancellationToken);
    }
}
