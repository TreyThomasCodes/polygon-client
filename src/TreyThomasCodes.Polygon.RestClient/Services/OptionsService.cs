// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;
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

    /// <summary>
    /// Initializes a new instance of the OptionsService with the specified API client and service provider.
    /// </summary>
    /// <param name="api">The Polygon.io options API client used for making HTTP requests.</param>
    /// <param name="serviceProvider">The service provider used to retrieve validators for request objects.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api or serviceProvider parameter is null.</exception>
    public OptionsService(IPolygonOptionsApi api, IServiceProvider serviceProvider)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<OptionsContract>> GetContractDetailsAsync(
        GetContractDetailsRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetContractDetailsRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetContractDetailsAsync(request.OptionsTicker, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<OptionSnapshot>> GetSnapshotAsync(
        GetSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetSnapshotRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetSnapshotAsync(request.UnderlyingAsset, request.OptionContract, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<List<OptionSnapshot>>> GetChainSnapshotAsync(
        GetChainSnapshotRequest request,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<PolygonResponse<OptionTrade>> GetLastTradeAsync(
        GetLastTradeRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetLastTradeRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetLastTradeAsync(request.OptionsTicker, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<List<OptionQuote>>> GetQuotesAsync(
        GetQuotesRequest request,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<PolygonResponse<List<OptionTradeV3>>> GetTradesAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<PolygonResponse<List<OptionBar>>> GetBarsAsync(
        GetBarsRequest request,
        CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task<OptionDailyOpenClose> GetDailyOpenCloseAsync(
        GetDailyOpenCloseRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetDailyOpenCloseRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetDailyOpenCloseAsync(request.OptionsTicker, request.Date, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PolygonResponse<List<OptionBar>>> GetPreviousDayBarAsync(
        GetPreviousDayBarRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<GetPreviousDayBarRequest>>();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await _api.GetPreviousDayBarAsync(request.OptionsTicker, request.Adjusted, cancellationToken);
    }
}
