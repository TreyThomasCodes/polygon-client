// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Stocks;

/// <summary>
/// Fluent query builder for constructing and executing aggregate bars (OHLC candlestick) requests.
/// This builder provides a progressive, chainable API for retrieving historical price data.
/// </summary>
public class BarsQueryBuilder
{
    private readonly IStocksService _service;
    private string? _ticker;
    private int? _multiplier;
    private AggregateInterval? _timespan;
    private string? _from;
    private string? _to;
    private bool? _adjusted;
    private SortOrder? _sort;
    private int? _limit;

    /// <summary>
    /// Initializes a new instance of the BarsQueryBuilder.
    /// </summary>
    /// <param name="service">The stocks service to execute the request against.</param>
    /// <param name="ticker">Optional initial ticker symbol.</param>
    public BarsQueryBuilder(IStocksService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ticker = ticker;
    }

    /// <summary>
    /// Specifies the ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder ForTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    /// <summary>
    /// Specifies the start date/timestamp for the query.
    /// </summary>
    /// <param name="from">Start date in YYYY-MM-DD format or timestamp.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder From(string from)
    {
        _from = from;
        return this;
    }

    /// <summary>
    /// Specifies the end date/timestamp for the query.
    /// </summary>
    /// <param name="to">End date in YYYY-MM-DD format or timestamp.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder To(string to)
    {
        _to = to;
        return this;
    }

    /// <summary>
    /// Specifies the aggregation interval using a multiplier and timespan.
    /// </summary>
    /// <param name="multiplier">The number of timespan units to aggregate (e.g., 1, 5, 15).</param>
    /// <param name="timespan">The timespan unit (minute, hour, day, week, month, quarter, year).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder WithInterval(int multiplier, AggregateInterval timespan)
    {
        _multiplier = multiplier;
        _timespan = timespan;
        return this;
    }

    /// <summary>
    /// Sets the aggregation to 1-minute bars.
    /// </summary>
    /// <param name="multiplier">Optional multiplier (defaults to 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Minute(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Minute);
    }

    /// <summary>
    /// Sets the aggregation to 1-hour bars.
    /// </summary>
    /// <param name="multiplier">Optional multiplier (defaults to 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Hourly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Hour);
    }

    /// <summary>
    /// Sets the aggregation to 1-day bars.
    /// </summary>
    /// <param name="multiplier">Optional multiplier (defaults to 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Daily(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Day);
    }

    /// <summary>
    /// Sets the aggregation to 1-week bars.
    /// </summary>
    /// <param name="multiplier">Optional multiplier (defaults to 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Weekly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Week);
    }

    /// <summary>
    /// Sets the aggregation to 1-month bars.
    /// </summary>
    /// <param name="multiplier">Optional multiplier (defaults to 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Monthly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Month);
    }

    /// <summary>
    /// Sets the aggregation to 1-quarter bars.
    /// </summary>
    /// <param name="multiplier">Optional multiplier (defaults to 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Quarterly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Quarter);
    }

    /// <summary>
    /// Sets the aggregation to 1-year bars.
    /// </summary>
    /// <param name="multiplier">Optional multiplier (defaults to 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Yearly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Year);
    }

    /// <summary>
    /// Specifies whether the results should be adjusted for stock splits.
    /// </summary>
    /// <param name="adjusted">True to adjust for splits, false otherwise. Defaults to true if not specified.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Adjusted(bool adjusted = true)
    {
        _adjusted = adjusted;
        return this;
    }

    /// <summary>
    /// Specifies the sort order for the results.
    /// </summary>
    /// <param name="sort">The sort order (ascending or descending by timestamp).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder SortBy(SortOrder sort)
    {
        _sort = sort;
        return this;
    }

    /// <summary>
    /// Sets the sort order to ascending (oldest first).
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Ascending()
    {
        return SortBy(SortOrder.Ascending);
    }

    /// <summary>
    /// Sets the sort order to descending (newest first).
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Descending()
    {
        return SortBy(SortOrder.Descending);
    }

    /// <summary>
    /// Limits the number of results returned.
    /// </summary>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BarsQueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the bars response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<List<StockBar>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_ticker))
            throw new InvalidOperationException("Ticker is required. Use ForTicker() to specify the ticker symbol.");

        if (!_multiplier.HasValue)
            throw new InvalidOperationException("Multiplier is required. Use WithInterval(), Daily(), Hourly(), or similar methods to specify the interval.");

        if (!_timespan.HasValue)
            throw new InvalidOperationException("Timespan is required. Use WithInterval(), Daily(), Hourly(), or similar methods to specify the interval.");

        if (string.IsNullOrWhiteSpace(_from))
            throw new InvalidOperationException("From date is required. Use From() to specify the start date.");

        if (string.IsNullOrWhiteSpace(_to))
            throw new InvalidOperationException("To date is required. Use To() to specify the end date.");

        var request = new GetBarsRequest
        {
            Ticker = _ticker,
            Multiplier = _multiplier.Value,
            Timespan = _timespan.Value,
            From = _from,
            To = _to,
            Adjusted = _adjusted,
            Sort = _sort,
            Limit = _limit
        };

        return _service.GetBarsAsync(request, cancellationToken);
    }
}
