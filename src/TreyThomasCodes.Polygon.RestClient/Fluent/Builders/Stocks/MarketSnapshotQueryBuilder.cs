// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Stocks;

/// <summary>
/// Fluent query builder for constructing and executing market snapshot requests.
/// This builder provides a progressive, chainable API for retrieving snapshots of all tickers.
/// </summary>
public class MarketSnapshotQueryBuilder
{
    private readonly IStocksService _service;
    private bool? _includeOtc;

    /// <summary>
    /// Initializes a new instance of the MarketSnapshotQueryBuilder.
    /// </summary>
    /// <param name="service">The stocks service to execute the request against.</param>
    public MarketSnapshotQueryBuilder(IStocksService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Specifies whether to include OTC (over-the-counter) securities.
    /// </summary>
    /// <param name="includeOtc">True to include OTC securities, false otherwise.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public MarketSnapshotQueryBuilder IncludeOtc(bool includeOtc = true)
    {
        _includeOtc = includeOtc;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the market snapshot response.</returns>
    public Task<PolygonResponse<List<StockSnapshot>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = new GetMarketSnapshotRequest
        {
            IncludeOtc = _includeOtc
        };

        return _service.GetMarketSnapshotAsync(request, cancellationToken);
    }
}
