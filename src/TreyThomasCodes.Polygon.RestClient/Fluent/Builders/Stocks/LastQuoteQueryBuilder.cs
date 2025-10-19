// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Stocks;

/// <summary>
/// Fluent query builder for constructing and executing last quote requests.
/// This builder provides a progressive, chainable API for retrieving the most recent NBBO quote for a stock.
/// </summary>
public class LastQuoteQueryBuilder
{
    private readonly IStocksService _service;
    private string? _ticker;

    /// <summary>
    /// Initializes a new instance of the LastQuoteQueryBuilder.
    /// </summary>
    /// <param name="service">The stocks service to execute the request against.</param>
    /// <param name="ticker">Optional initial ticker symbol.</param>
    public LastQuoteQueryBuilder(IStocksService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ticker = ticker;
    }

    /// <summary>
    /// Specifies the ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public LastQuoteQueryBuilder ForTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the most recent quote.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the last quote response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<LastQuoteResult>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_ticker))
            throw new InvalidOperationException("Ticker is required. Use ForTicker() to specify the ticker symbol.");

        var request = new GetLastQuoteRequest { Ticker = _ticker };
        return _service.GetLastQuoteAsync(request, cancellationToken);
    }
}
