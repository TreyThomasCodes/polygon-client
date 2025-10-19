// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Options;

/// <summary>
/// Fluent query builder for constructing and executing options daily open/close requests.
/// This builder provides a progressive, chainable API for retrieving comprehensive daily trading data.
/// </summary>
public class OptionDailyOpenCloseQueryBuilder
{
    private readonly IOptionsService _service;
    private string? _optionsTicker;
    private string? _date;

    /// <summary>
    /// Initializes a new instance of the OptionDailyOpenCloseQueryBuilder.
    /// </summary>
    /// <param name="service">The options service to execute the request against.</param>
    /// <param name="ticker">Optional initial options ticker symbol.</param>
    public OptionDailyOpenCloseQueryBuilder(IOptionsService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _optionsTicker = ticker;
    }

    /// <summary>
    /// Specifies the options ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The options ticker symbol in OCC format (e.g., "O:SPY251219C00650000").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionDailyOpenCloseQueryBuilder ForTicker(string ticker)
    {
        _optionsTicker = ticker;
        return this;
    }

    /// <summary>
    /// Specifies the date for which to retrieve daily data.
    /// </summary>
    /// <param name="date">The date in YYYY-MM-DD format (e.g., "2023-01-09").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionDailyOpenCloseQueryBuilder OnDate(string date)
    {
        _date = date;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the daily open/close response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<OptionDailyOpenClose> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_optionsTicker))
            throw new InvalidOperationException("Options ticker is required. Use ForTicker() to specify the options ticker symbol.");

        if (string.IsNullOrWhiteSpace(_date))
            throw new InvalidOperationException("Date is required. Use OnDate() to specify the date.");

        var request = new GetDailyOpenCloseRequest
        {
            OptionsTicker = _optionsTicker,
            Date = _date
        };

        return _service.GetDailyOpenCloseAsync(request, cancellationToken);
    }
}
