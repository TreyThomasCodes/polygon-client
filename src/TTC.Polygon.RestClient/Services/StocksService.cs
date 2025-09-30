// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TTC.Polygon.RestClient.Api;
using TTC.Polygon.Models.Common;
using TTC.Polygon.Models.Stocks;

namespace TTC.Polygon.RestClient.Services;

/// <summary>
/// Implementation of the stocks service for accessing Polygon.io stocks market data.
/// Provides methods to retrieve real-time and historical stock trades, quotes, snapshots, and OHLC aggregates.
/// </summary>
public class StocksService : IStocksService
{
    private readonly IPolygonStocksApi _api;

    /// <summary>
    /// Initializes a new instance of the StocksService with the specified API client.
    /// </summary>
    /// <param name="api">The Polygon.io stocks API client used for making HTTP requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api parameter is null.</exception>
    public StocksService(IPolygonStocksApi api)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<StockBar>>> GetBarsAsync(
        string ticker,
        int multiplier,
        string timespan,
        string from,
        string to,
        bool? adjusted = null,
        string? sort = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetBarsAsync(
            ticker,
            multiplier,
            timespan,
            from,
            to,
            adjusted,
            sort,
            limit,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<StockBar>>> GetPreviousCloseAsync(
        string ticker,
        bool? adjusted = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetPreviousCloseAsync(ticker, adjusted, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<StockBar>>> GetGroupedDailyAsync(
        string date,
        bool? adjusted = null,
        bool? includeOtc = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetGroupedDailyAsync(date, adjusted, includeOtc, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<StockBar>>> GetDailyOpenCloseAsync(
        string ticker,
        string date,
        bool? adjusted = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetDailyOpenCloseAsync(ticker, date, adjusted, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<StockTrade>>> GetTradesAsync(
        string ticker,
        string? timestamp = null,
        string? timestampGte = null,
        string? timestampGt = null,
        string? timestampLte = null,
        string? timestampLt = null,
        int? limit = null,
        string? sort = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetTradesAsync(
            ticker,
            timestamp,
            timestampGte,
            timestampGt,
            timestampLte,
            timestampLt,
            limit,
            sort,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<StockQuote>>> GetQuotesAsync(
        string ticker,
        string? timestamp = null,
        string? timestampGte = null,
        string? timestampGt = null,
        string? timestampLte = null,
        string? timestampLt = null,
        int? limit = null,
        string? sort = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetQuotesAsync(
            ticker,
            timestamp,
            timestampGte,
            timestampGt,
            timestampLte,
            timestampLt,
            limit,
            sort,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<StockSnapshot>>> GetMarketSnapshotAsync(
        bool? includeOtc = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetMarketSnapshotAsync(includeOtc, cancellationToken);
    }

    /// <inheritdoc />
    public Task<StockSnapshotResponse> GetSnapshotAsync(
        string ticker,
        CancellationToken cancellationToken = default)
    {
        return _api.GetSnapshotAsync(ticker, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<StockTrade>> GetLastTradeAsync(
        string ticker,
        CancellationToken cancellationToken = default)
    {
        return _api.GetLastTradeAsync(ticker, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<LastQuoteResult>> GetLastQuoteAsync(
        string ticker,
        CancellationToken cancellationToken = default)
    {
        return _api.GetLastQuoteAsync(ticker, cancellationToken);
    }
}