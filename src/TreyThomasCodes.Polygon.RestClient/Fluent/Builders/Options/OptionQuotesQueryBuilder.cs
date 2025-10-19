// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Options;

/// <summary>
/// Fluent query builder for constructing and executing options quote (bid/ask) data requests.
/// This builder provides a progressive, chainable API for retrieving tick-level quote data for options contracts.
/// </summary>
public class OptionQuotesQueryBuilder
{
    private readonly IOptionsService _service;
    private string? _optionsTicker;
    private string? _timestamp;
    private string? _timestampGte;
    private string? _timestampGt;
    private string? _timestampLte;
    private string? _timestampLt;
    private int? _limit;
    private string? _order;
    private string? _sort;
    private string? _cursor;

    /// <summary>
    /// Initializes a new instance of the OptionQuotesQueryBuilder.
    /// </summary>
    /// <param name="service">The options service to execute the request against.</param>
    /// <param name="ticker">Optional initial options ticker symbol.</param>
    public OptionQuotesQueryBuilder(IOptionsService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _optionsTicker = ticker;
    }

    /// <summary>
    /// Specifies the options ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The options ticker symbol in OCC format (e.g., "O:SPY241220P00720000").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder ForTicker(string ticker)
    {
        _optionsTicker = ticker;
        return this;
    }

    /// <summary>
    /// Filters quotes to those at or after this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder AtOrAfter(string timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    /// <summary>
    /// Filters quotes to those greater than or equal to this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder GreaterThanOrEqual(string timestamp)
    {
        _timestampGte = timestamp;
        return this;
    }

    /// <summary>
    /// Filters quotes to those strictly after this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder GreaterThan(string timestamp)
    {
        _timestampGt = timestamp;
        return this;
    }

    /// <summary>
    /// Filters quotes to those less than or equal to this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder LessThanOrEqual(string timestamp)
    {
        _timestampLte = timestamp;
        return this;
    }

    /// <summary>
    /// Filters quotes to those strictly before this timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp in YYYY-MM-DD, YYYY-MM-DDTHH:MM:SS, or nanosecond format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder LessThan(string timestamp)
    {
        _timestampLt = timestamp;
        return this;
    }

    /// <summary>
    /// Filters quotes within a time range (inclusive).
    /// </summary>
    /// <param name="from">Start timestamp.</param>
    /// <param name="to">End timestamp.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder Between(string from, string to)
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
    public OptionQuotesQueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    /// <summary>
    /// Specifies the field to sort results by.
    /// </summary>
    /// <param name="field">The field name to sort by. Defaults to "timestamp".</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder SortBy(string field)
    {
        _sort = field;
        return this;
    }

    /// <summary>
    /// Sets the sort order to ascending (oldest first).
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder Ascending()
    {
        _order = "asc";
        return this;
    }

    /// <summary>
    /// Sets the sort order to descending (newest first).
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder Descending()
    {
        _order = "desc";
        return this;
    }

    /// <summary>
    /// Specifies the pagination cursor for retrieving subsequent pages of results.
    /// </summary>
    /// <param name="cursor">The cursor value from a previous response's next_url.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionQuotesQueryBuilder WithCursor(string cursor)
    {
        _cursor = cursor;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the quotes response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<List<OptionQuote>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_optionsTicker))
            throw new InvalidOperationException("Options ticker is required. Use ForTicker() to specify the options ticker symbol.");

        var request = new GetQuotesRequest
        {
            OptionsTicker = _optionsTicker,
            Timestamp = _timestamp,
            TimestampGte = _timestampGte,
            TimestampGt = _timestampGt,
            TimestampLte = _timestampLte,
            TimestampLt = _timestampLt,
            Limit = _limit,
            Order = _order,
            Sort = _sort,
            Cursor = _cursor
        };

        return _service.GetQuotesAsync(request, cancellationToken);
    }
}
