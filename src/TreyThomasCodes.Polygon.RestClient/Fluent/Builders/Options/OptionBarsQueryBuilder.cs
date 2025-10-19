// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Options;

/// <summary>
/// Fluent query builder for constructing and executing options aggregate bars (OHLC) requests.
/// This builder provides a progressive, chainable API for retrieving historical pricing data aggregated by time interval.
/// </summary>
public class OptionBarsQueryBuilder
{
    private readonly IOptionsService _service;
    private string? _optionsTicker;
    private int? _multiplier;
    private AggregateInterval? _timespan;
    private string? _from;
    private string? _to;
    private bool? _adjusted;
    private SortOrder? _sort;
    private int? _limit;

    /// <summary>
    /// Initializes a new instance of the OptionBarsQueryBuilder.
    /// </summary>
    /// <param name="service">The options service to execute the request against.</param>
    /// <param name="ticker">Optional initial options ticker symbol.</param>
    public OptionBarsQueryBuilder(IOptionsService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _optionsTicker = ticker;
    }

    /// <summary>
    /// Specifies the options ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The options ticker symbol in OCC format (e.g., "O:SPY251219C00650000").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder ForTicker(string ticker)
    {
        _optionsTicker = ticker;
        return this;
    }

    /// <summary>
    /// Specifies the start date for the aggregate window.
    /// </summary>
    /// <param name="from">The start date in YYYY-MM-DD format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder From(string from)
    {
        _from = from;
        return this;
    }

    /// <summary>
    /// Specifies the end date for the aggregate window.
    /// </summary>
    /// <param name="to">The end date in YYYY-MM-DD format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder To(string to)
    {
        _to = to;
        return this;
    }

    /// <summary>
    /// Sets the interval to minute bars with the specified multiplier.
    /// </summary>
    /// <param name="multiplier">The number of minutes per bar (e.g., 1, 5, 15).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Minutely(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Minute);
    }

    /// <summary>
    /// Sets the interval to hourly bars with the specified multiplier.
    /// </summary>
    /// <param name="multiplier">The number of hours per bar (e.g., 1, 4).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Hourly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Hour);
    }

    /// <summary>
    /// Sets the interval to daily bars with the specified multiplier.
    /// </summary>
    /// <param name="multiplier">The number of days per bar (default is 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Daily(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Day);
    }

    /// <summary>
    /// Sets the interval to weekly bars with the specified multiplier.
    /// </summary>
    /// <param name="multiplier">The number of weeks per bar (default is 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Weekly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Week);
    }

    /// <summary>
    /// Sets the interval to monthly bars with the specified multiplier.
    /// </summary>
    /// <param name="multiplier">The number of months per bar (default is 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Monthly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Month);
    }

    /// <summary>
    /// Sets the interval to quarterly bars with the specified multiplier.
    /// </summary>
    /// <param name="multiplier">The number of quarters per bar (default is 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Quarterly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Quarter);
    }

    /// <summary>
    /// Sets the interval to yearly bars with the specified multiplier.
    /// </summary>
    /// <param name="multiplier">The number of years per bar (default is 1).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Yearly(int multiplier = 1)
    {
        return WithInterval(multiplier, AggregateInterval.Year);
    }

    /// <summary>
    /// Sets a custom aggregation interval.
    /// </summary>
    /// <param name="multiplier">The number of timespan units to aggregate.</param>
    /// <param name="timespan">The timespan unit (minute, hour, day, week, month, quarter, year).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder WithInterval(int multiplier, AggregateInterval timespan)
    {
        _multiplier = multiplier;
        _timespan = timespan;
        return this;
    }

    /// <summary>
    /// Specifies whether the results should be adjusted for splits.
    /// </summary>
    /// <param name="adjusted">True to adjust for splits, false otherwise.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Adjusted(bool adjusted = true)
    {
        _adjusted = adjusted;
        return this;
    }

    /// <summary>
    /// Sets the sort order to ascending (oldest first).
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Ascending()
    {
        _sort = SortOrder.Ascending;
        return this;
    }

    /// <summary>
    /// Sets the sort order to descending (newest first).
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Descending()
    {
        _sort = SortOrder.Descending;
        return this;
    }

    /// <summary>
    /// Limits the number of results returned.
    /// </summary>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionBarsQueryBuilder Limit(int limit)
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
    public Task<PolygonResponse<List<OptionBar>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_optionsTicker))
            throw new InvalidOperationException("Options ticker is required. Use ForTicker() to specify the options ticker symbol.");

        if (!_multiplier.HasValue)
            throw new InvalidOperationException("Multiplier is required. Use one of the interval methods (e.g., Daily(), Hourly(), WithInterval()).");

        if (!_timespan.HasValue)
            throw new InvalidOperationException("Timespan is required. Use one of the interval methods (e.g., Daily(), Hourly(), WithInterval()).");

        if (string.IsNullOrWhiteSpace(_from))
            throw new InvalidOperationException("From date is required. Use From() to specify the start date.");

        if (string.IsNullOrWhiteSpace(_to))
            throw new InvalidOperationException("To date is required. Use To() to specify the end date.");

        var request = new GetBarsRequest
        {
            OptionsTicker = _optionsTicker,
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
