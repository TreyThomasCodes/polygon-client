// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Stocks;

/// <summary>
/// Fluent query builder for constructing and executing trade data requests.
/// This builder provides a progressive, chainable API for retrieving tick-level trade data.
/// </summary>
public class TradesQueryBuilder
{
    private readonly IStocksService _service;
    private string? _ticker;
    private string? _timestamp;
    private string? _timestampGte;
    private string? _timestampGt;
    private string? _timestampLte;
    private string? _timestampLt;
    private int? _limit;
    private string? _sort;

    /// <summary>
    /// Initializes a new instance of the TradesQueryBuilder.
    /// </summary>
    /// <param name="service">The stocks service to execute the request against.</param>
    /// <param name="ticker">Optional initial ticker symbol.</param>
    public TradesQueryBuilder(IStocksService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ticker = ticker;
    }

    /// <summary>
    /// Specifies the ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder ForTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    /// <summary>
    /// Filters trades to those at or after this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder AtOrAfter(string timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    /// <summary>
    /// Filters trades to those greater than or equal to this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder GreaterThanOrEqual(string timestamp)
    {
        _timestampGte = timestamp;
        return this;
    }

    /// <summary>
    /// Filters trades to those strictly after this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder GreaterThan(string timestamp)
    {
        _timestampGt = timestamp;
        return this;
    }

    /// <summary>
    /// Filters trades to those less than or equal to this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder LessThanOrEqual(string timestamp)
    {
        _timestampLte = timestamp;
        return this;
    }

    /// <summary>
    /// Filters trades to those strictly before this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder LessThan(string timestamp)
    {
        _timestampLt = timestamp;
        return this;
    }

    /// <summary>
    /// Filters trades within a time range (inclusive).
    /// </summary>
    /// <param name="from">Start timestamp.</param>
    /// <param name="to">End timestamp.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder Between(string from, string to)
    {
        _timestampGte = from;
        _timestampLte = to;
        return this;
    }

    /// <summary>
    /// Limits the number of results returned.
    /// </summary>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    /// <summary>
    /// Specifies the sort order for the results.
    /// </summary>
    /// <param name="sort">The sort order string value (e.g., "asc", "desc").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder SortBy(string sort)
    {
        _sort = sort;
        return this;
    }

    /// <summary>
    /// Sets the sort order to ascending (oldest first).
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder Ascending()
    {
        return SortBy("asc");
    }

    /// <summary>
    /// Sets the sort order to descending (newest first).
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public TradesQueryBuilder Descending()
    {
        return SortBy("desc");
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the trades response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<List<StockTrade>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_ticker))
            throw new InvalidOperationException("Ticker is required. Use ForTicker() to specify the ticker symbol.");

        var request = new GetTradesRequest
        {
            Ticker = _ticker,
            Timestamp = _timestamp,
            TimestampGte = _timestampGte,
            TimestampGt = _timestampGt,
            TimestampLte = _timestampLte,
            TimestampLt = _timestampLt,
            Limit = _limit,
            Sort = _sort
        };

        return _service.GetTradesAsync(request, cancellationToken);
    }
}
