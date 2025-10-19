// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Stocks;

/// <summary>
/// Fluent query builder for constructing and executing grouped daily aggregate requests.
/// This builder provides a progressive, chainable API for retrieving daily aggregates for all tickers.
/// </summary>
public class GroupedDailyQueryBuilder
{
    private readonly IStocksService _service;
    private string? _date;
    private bool? _adjusted;
    private bool? _includeOtc;

    /// <summary>
    /// Initializes a new instance of the GroupedDailyQueryBuilder.
    /// </summary>
    /// <param name="service">The stocks service to execute the request against.</param>
    /// <param name="date">Optional initial date (YYYY-MM-DD format).</param>
    public GroupedDailyQueryBuilder(IStocksService service, string? date = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _date = date;
    }

    /// <summary>
    /// Specifies the date for the query.
    /// </summary>
    /// <param name="date">The date in YYYY-MM-DD format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public GroupedDailyQueryBuilder ForDate(string date)
    {
        _date = date;
        return this;
    }

    /// <summary>
    /// Specifies whether the results should be adjusted for stock splits.
    /// </summary>
    /// <param name="adjusted">True to adjust for splits, false otherwise.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public GroupedDailyQueryBuilder Adjusted(bool adjusted = true)
    {
        _adjusted = adjusted;
        return this;
    }

    /// <summary>
    /// Specifies whether to include OTC (over-the-counter) securities.
    /// </summary>
    /// <param name="includeOtc">True to include OTC securities, false otherwise.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public GroupedDailyQueryBuilder IncludeOtc(bool includeOtc = true)
    {
        _includeOtc = includeOtc;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the grouped daily response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<List<StockBar>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_date))
            throw new InvalidOperationException("Date is required. Use ForDate() to specify the date.");

        var request = new GetGroupedDailyRequest
        {
            Date = _date,
            Adjusted = _adjusted,
            IncludeOtc = _includeOtc
        };

        return _service.GetGroupedDailyAsync(request, cancellationToken);
    }
}
