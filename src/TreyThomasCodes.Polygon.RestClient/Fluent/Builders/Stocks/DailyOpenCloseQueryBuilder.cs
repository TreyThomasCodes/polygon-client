// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Stocks;

/// <summary>
/// Fluent query builder for constructing and executing daily open/close requests.
/// This builder provides a progressive, chainable API for retrieving daily OHLC data.
/// </summary>
public class DailyOpenCloseQueryBuilder
{
    private readonly IStocksService _service;
    private string? _ticker;
    private string? _date;
    private bool? _adjusted;

    /// <summary>
    /// Initializes a new instance of the DailyOpenCloseQueryBuilder.
    /// </summary>
    /// <param name="service">The stocks service to execute the request against.</param>
    /// <param name="ticker">Optional initial ticker symbol.</param>
    public DailyOpenCloseQueryBuilder(IStocksService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ticker = ticker;
    }

    /// <summary>
    /// Specifies the ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public DailyOpenCloseQueryBuilder ForTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    /// <summary>
    /// Specifies the date for the query.
    /// </summary>
    /// <param name="date">The date in YYYY-MM-DD format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public DailyOpenCloseQueryBuilder ForDate(string date)
    {
        _date = date;
        return this;
    }

    /// <summary>
    /// Specifies whether the results should be adjusted for stock splits.
    /// </summary>
    /// <param name="adjusted">True to adjust for splits, false otherwise.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public DailyOpenCloseQueryBuilder Adjusted(bool adjusted = true)
    {
        _adjusted = adjusted;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the daily open/close response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<List<StockBar>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_ticker))
            throw new InvalidOperationException("Ticker is required. Use ForTicker() to specify the ticker symbol.");

        if (string.IsNullOrWhiteSpace(_date))
            throw new InvalidOperationException("Date is required. Use ForDate() to specify the date.");

        var request = new GetDailyOpenCloseRequest
        {
            Ticker = _ticker,
            Date = _date,
            Adjusted = _adjusted
        };

        return _service.GetDailyOpenCloseAsync(request, cancellationToken);
    }
}
