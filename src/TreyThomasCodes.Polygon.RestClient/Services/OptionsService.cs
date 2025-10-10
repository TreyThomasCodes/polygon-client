// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Api;

namespace TreyThomasCodes.Polygon.RestClient.Services;

/// <summary>
/// Implementation of the options service for accessing Polygon.io options market data.
/// Provides methods to retrieve options contract information, trades, quotes, snapshots, and OHLC aggregates.
/// This service acts as a wrapper around the Polygon.io Options API, providing a convenient interface for accessing options data.
/// </summary>
public class OptionsService : IOptionsService
{
    private readonly IPolygonOptionsApi _api;

    /// <summary>
    /// Initializes a new instance of the OptionsService with the specified API client.
    /// </summary>
    /// <param name="api">The Polygon.io options API client used for making HTTP requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api parameter is null.</exception>
    public OptionsService(IPolygonOptionsApi api)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
    }

    /// <inheritdoc />
    public Task<PolygonResponse<OptionsContract>> GetContractDetailsAsync(
        string optionsTicker,
        CancellationToken cancellationToken = default)
    {
        return _api.GetContractDetailsAsync(optionsTicker, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<OptionSnapshot>> GetSnapshotAsync(
        string underlyingAsset,
        string optionContract,
        CancellationToken cancellationToken = default)
    {
        return _api.GetSnapshotAsync(underlyingAsset, optionContract, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<OptionSnapshot>>> GetChainSnapshotAsync(
        string underlyingAsset,
        decimal? strikePrice = null,
        string? contractType = null,
        string? expirationDateGte = null,
        string? expirationDateLte = null,
        int? limit = null,
        string? order = null,
        string? sort = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        return _api.GetChainSnapshotAsync(
            underlyingAsset,
            strikePrice,
            contractType,
            expirationDateGte,
            expirationDateLte,
            limit,
            order,
            sort,
            cursor,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<OptionTrade>> GetLastTradeAsync(
        string optionsTicker,
        CancellationToken cancellationToken = default)
    {
        return _api.GetLastTradeAsync(optionsTicker, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<OptionQuote>>> GetQuotesAsync(
        string optionsTicker,
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
        return _api.GetQuotesAsync(
            optionsTicker,
            timestamp,
            timestampLt,
            timestampLte,
            timestampGt,
            timestampGte,
            order,
            limit,
            sort,
            cursor,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<PolygonResponse<List<OptionTradeV3>>> GetTradesAsync(
        string optionsTicker,
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
        return _api.GetTradesAsync(
            optionsTicker,
            timestamp,
            timestampLt,
            timestampLte,
            timestampGt,
            timestampGte,
            order,
            limit,
            sort,
            cursor,
            cancellationToken);
    }
}
