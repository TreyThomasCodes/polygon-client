// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Reference;

/// <summary>
/// Fluent query builder for constructing and executing ticker details requests.
/// This builder provides a progressive, chainable API for retrieving comprehensive ticker information.
/// </summary>
public class TickerDetailsQueryBuilder
{
    private readonly IReferenceDataService _service;
    private string? _ticker;
    private string? _date;

    /// <summary>
    /// Initializes a new instance of the TickerDetailsQueryBuilder.
    /// </summary>
    /// <param name="service">The reference data service to execute the request against.</param>
    /// <param name="ticker">Optional initial ticker symbol.</param>
    public TickerDetailsQueryBuilder(IReferenceDataService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ticker = ticker;
    }

    /// <summary>
    /// Specifies the ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickerDetailsQueryBuilder ForTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    /// <summary>
    /// Specifies the date to retrieve ticker details as of.
    /// </summary>
    /// <param name="date">The date in YYYY-MM-DD format. Defaults to the most recent available data if not specified.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickerDetailsQueryBuilder AsOf(string date)
    {
        _date = date;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the ticker details response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<StockTicker>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_ticker))
            throw new InvalidOperationException("Ticker is required. Use ForTicker() to specify the ticker symbol.");

        var request = new GetTickerDetailsRequest
        {
            Ticker = _ticker,
            Date = _date
        };

        return _service.GetTickerDetailsAsync(request, cancellationToken);
    }
}
