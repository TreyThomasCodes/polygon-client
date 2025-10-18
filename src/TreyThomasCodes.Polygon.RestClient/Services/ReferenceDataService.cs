// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TreyThomasCodes.Polygon.RestClient.Api;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

namespace TreyThomasCodes.Polygon.RestClient.Services;

/// <summary>
/// Implementation of the reference data service for accessing Polygon.io reference data and market status information.
/// Provides methods to retrieve ticker information, market metadata, trading status, and other reference information.
/// </summary>
public class ReferenceDataService : IReferenceDataService
{
    private readonly IPolygonReferenceApi _api;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the ReferenceDataService with the specified API client and service provider.
    /// </summary>
    /// <param name="api">The Polygon.io reference API client used for making HTTP requests.</param>
    /// <param name="serviceProvider">The service provider used to retrieve validators for request objects.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api or serviceProvider parameter is null.</exception>
    public ReferenceDataService(IPolygonReferenceApi api, IServiceProvider serviceProvider)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<List<StockTicker>>> GetTickersAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<PolygonResponse<StockTicker>> GetTickerDetailsAsync(
        GetTickerDetailsRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetTickerDetailsRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetTickerDetailsAsync(request.Ticker, request.Date, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<MarketStatus> GetMarketStatusAsync(
        GetMarketStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetMarketStatusRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetMarketStatusAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TickerTypesResponse> GetTickerTypesAsync(
        GetTickerTypesRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetTickerTypesRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetTickerTypesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<List<ConditionCode>>> GetConditionCodesAsync(
        GetConditionCodesRequest request,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<PolygonResponse<List<Exchange>>> GetExchangesAsync(
        GetExchangesRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetExchangesRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetExchangesAsync(request.AssetClass, request.Locale, cancellationToken);
    }
}
