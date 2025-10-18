// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TreyThomasCodes.Polygon.RestClient.Api;
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

    /// <summary>
    /// Initializes a new instance of the StocksService with the specified API client and service provider.
    /// </summary>
    /// <param name="api">The Polygon.io stocks API client used for making HTTP requests.</param>
    /// <param name="serviceProvider">The service provider used to retrieve validators for request objects.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api or serviceProvider parameter is null.</exception>
    public StocksService(IPolygonStocksApi api, IServiceProvider serviceProvider)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<List<StockBar>>> GetBarsAsync(
        GetBarsRequest request,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<PolygonResponse<List<StockBar>>> GetPreviousCloseAsync(
        GetPreviousCloseRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetPreviousCloseRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetPreviousCloseAsync(request.Ticker, request.Adjusted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<List<StockBar>>> GetGroupedDailyAsync(
        GetGroupedDailyRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetGroupedDailyRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetGroupedDailyAsync(request.Date, request.Adjusted, request.IncludeOtc, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<List<StockBar>>> GetDailyOpenCloseAsync(
        GetDailyOpenCloseRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetDailyOpenCloseRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetDailyOpenCloseAsync(request.Ticker, request.Date, request.Adjusted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<List<StockTrade>>> GetTradesAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<PolygonResponse<List<StockQuote>>> GetQuotesAsync(
        GetQuotesRequest request,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<PolygonResponse<List<StockSnapshot>>> GetMarketSnapshotAsync(
        GetMarketSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetMarketSnapshotRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetMarketSnapshotAsync(request.IncludeOtc, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<StockSnapshotResponse> GetSnapshotAsync(
        GetSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetSnapshotRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetSnapshotAsync(request.Ticker, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<StockTrade>> GetLastTradeAsync(
        GetLastTradeRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetLastTradeRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetLastTradeAsync(request.Ticker, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<LastQuoteResult>> GetLastQuoteAsync(
        GetLastQuoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetLastQuoteRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetLastQuoteAsync(request.Ticker, cancellationToken);
    }
}
