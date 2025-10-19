// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Options;

/// <summary>
/// Fluent query builder for constructing and executing options chain snapshot requests.
/// This builder provides a progressive, chainable API for retrieving market data for all options contracts on an underlying asset.
/// </summary>
public class ChainSnapshotQueryBuilder
{
    private readonly IOptionsService _service;
    private string? _underlyingAsset;
    private decimal? _strikePrice;
    private string? _contractType;
    private string? _expirationDateGte;
    private string? _expirationDateLte;
    private int? _limit;
    private string? _order;
    private string? _sort;
    private string? _cursor;

    /// <summary>
    /// Initializes a new instance of the ChainSnapshotQueryBuilder.
    /// </summary>
    /// <param name="service">The options service to execute the request against.</param>
    /// <param name="underlying">Optional initial underlying asset ticker symbol.</param>
    public ChainSnapshotQueryBuilder(IOptionsService service, string? underlying = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _underlyingAsset = underlying;
    }

    /// <summary>
    /// Specifies the underlying asset ticker symbol for the query.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY", "AAPL", "MSTR").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder ForUnderlying(string underlying)
    {
        _underlyingAsset = underlying;
        return this;
    }

    /// <summary>
    /// Filters the results to only include contracts with the specified strike price.
    /// </summary>
    /// <param name="strike">The exact strike price to filter by.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder AtStrike(decimal strike)
    {
        _strikePrice = strike;
        return this;
    }

    /// <summary>
    /// Filters the results to only include call options.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder CallsOnly()
    {
        _contractType = "call";
        return this;
    }

    /// <summary>
    /// Filters the results to only include put options.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder PutsOnly()
    {
        _contractType = "put";
        return this;
    }

    /// <summary>
    /// Filters the results to only include contracts expiring on or after the specified date.
    /// </summary>
    /// <param name="date">The minimum expiration date in YYYY-MM-DD format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder ExpiringOnOrAfter(string date)
    {
        _expirationDateGte = date;
        return this;
    }

    /// <summary>
    /// Filters the results to only include contracts expiring on or before the specified date.
    /// </summary>
    /// <param name="date">The maximum expiration date in YYYY-MM-DD format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder ExpiringOnOrBefore(string date)
    {
        _expirationDateLte = date;
        return this;
    }

    /// <summary>
    /// Filters the results to only include contracts expiring within the specified date range.
    /// </summary>
    /// <param name="from">The minimum expiration date in YYYY-MM-DD format.</param>
    /// <param name="to">The maximum expiration date in YYYY-MM-DD format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder ExpiringBetween(string from, string to)
    {
        _expirationDateGte = from;
        _expirationDateLte = to;
        return this;
    }

    /// <summary>
    /// Limits the number of results returned.
    /// </summary>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    /// <summary>
    /// Specifies the field to sort results by.
    /// </summary>
    /// <param name="field">The field name to sort by (e.g., "ticker", "strike_price", "expiration_date").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder SortBy(string field)
    {
        _sort = field;
        return this;
    }

    /// <summary>
    /// Sets the sort order to ascending.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder Ascending()
    {
        _order = "asc";
        return this;
    }

    /// <summary>
    /// Sets the sort order to descending.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder Descending()
    {
        _order = "desc";
        return this;
    }

    /// <summary>
    /// Specifies the pagination cursor for retrieving subsequent pages of results.
    /// </summary>
    /// <param name="cursor">The cursor value from a previous response's next_url.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ChainSnapshotQueryBuilder WithCursor(string cursor)
    {
        _cursor = cursor;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the chain snapshot response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<List<OptionSnapshot>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_underlyingAsset))
            throw new InvalidOperationException("Underlying asset is required. Use ForUnderlying() to specify the underlying asset ticker symbol.");

        var request = new GetChainSnapshotRequest
        {
            UnderlyingAsset = _underlyingAsset,
            StrikePrice = _strikePrice,
            ContractType = _contractType,
            ExpirationDateGte = _expirationDateGte,
            ExpirationDateLte = _expirationDateLte,
            Limit = _limit,
            Order = _order,
            Sort = _sort,
            Cursor = _cursor
        };

        return _service.GetChainSnapshotAsync(request, cancellationToken);
    }
}
