// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TTC.Polygon.RestClient.Api;
using TTC.Polygon.Models.Reference;
using TTC.Polygon.Models.Common;
using TTC.Polygon.Models.Stocks;

namespace TTC.Polygon.RestClient.Services;

/// <summary>
/// Implementation of the reference data service for accessing Polygon.io reference data and market status information.
/// Provides methods to retrieve ticker information, market metadata, trading status, and other reference information.
/// </summary>
public class ReferenceDataService : IReferenceDataService
{
    private readonly IPolygonReferenceApi _api;

    /// <summary>
    /// Initializes a new instance of the ReferenceDataService with the specified API client.
    /// </summary>
    /// <param name="api">The Polygon.io reference API client used for making HTTP requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api parameter is null.</exception>
    public ReferenceDataService(IPolygonReferenceApi api)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<StockTicker>>> GetTickersAsync(
        string? ticker = null,
        string? tickerGt = null,
        string? tickerGte = null,
        string? tickerLt = null,
        string? tickerLte = null,
        string? type = null,
        string? market = null,
        string? exchange = null,
        string? cusip = null,
        string? cik = null,
        string? date = null,
        string? search = null,
        bool? active = null,
        string? sort = null,
        string? order = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetTickersAsync(
            ticker,
            tickerGt,
            tickerGte,
            tickerLt,
            tickerLte,
            type,
            market,
            exchange,
            cusip,
            cik,
            date,
            search,
            active,
            sort,
            order,
            limit,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<StockTicker>> GetTickerDetailsAsync(
        string ticker,
        string? date = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetTickerDetailsAsync(ticker, date, cancellationToken);
    }

    /// <inheritdoc />
    public Task<MarketStatus> GetMarketStatusAsync(CancellationToken cancellationToken = default)
    {
        return _api.GetMarketStatusAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TickerTypesResponse> GetTickerTypesAsync(CancellationToken cancellationToken = default)
    {
        return _api.GetTickerTypesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<ConditionCode>>> GetConditionCodesAsync(
        string? assetClass = null,
        string? dataType = null,
        string? id = null,
        string? sipMapping = null,
        string? order = null,
        int? limit = null,
        string? sort = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetConditionCodesAsync(
            assetClass,
            dataType,
            id,
            sipMapping,
            order,
            limit,
            sort,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<Exchange>>> GetExchangesAsync(
        string? assetClass = null,
        string? locale = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetExchangesAsync(assetClass, locale, cancellationToken);
    }
}