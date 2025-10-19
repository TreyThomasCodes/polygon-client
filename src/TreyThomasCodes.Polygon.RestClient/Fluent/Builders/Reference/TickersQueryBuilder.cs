// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Reference;

/// <summary>
/// Fluent query builder for constructing and executing tickers list requests.
/// This builder provides a progressive, chainable API for searching and filtering ticker symbols.
/// </summary>
public class TickersQueryBuilder
{
    private readonly IReferenceDataService _service;
    private string? _ticker;
    private string? _tickerGt;
    private string? _tickerGte;
    private string? _tickerLt;
    private string? _tickerLte;
    private string? _type;
    private Market? _market;
    private string? _exchange;
    private string? _cusip;
    private string? _cik;
    private string? _date;
    private string? _search;
    private bool? _active;
    private string? _sort;
    private SortOrder? _order;
    private int? _limit;

    /// <summary>
    /// Initializes a new instance of the TickersQueryBuilder.
    /// </summary>
    /// <param name="service">The reference data service to execute the request against.</param>
    public TickersQueryBuilder(IReferenceDataService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Filters for an exact ticker symbol.
    /// </summary>
    /// <param name="ticker">The exact ticker symbol (e.g., "AAPL").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder WithTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    /// <summary>
    /// Filters for tickers alphabetically greater than the specified value.
    /// </summary>
    /// <param name="ticker">The ticker value to compare against.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder GreaterThan(string ticker)
    {
        _tickerGt = ticker;
        return this;
    }

    /// <summary>
    /// Filters for tickers alphabetically greater than or equal to the specified value.
    /// </summary>
    /// <param name="ticker">The ticker value to compare against.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder GreaterThanOrEqual(string ticker)
    {
        _tickerGte = ticker;
        return this;
    }

    /// <summary>
    /// Filters for tickers alphabetically less than the specified value.
    /// </summary>
    /// <param name="ticker">The ticker value to compare against.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder LessThan(string ticker)
    {
        _tickerLt = ticker;
        return this;
    }

    /// <summary>
    /// Filters for tickers alphabetically less than or equal to the specified value.
    /// </summary>
    /// <param name="ticker">The ticker value to compare against.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder LessThanOrEqual(string ticker)
    {
        _tickerLte = ticker;
        return this;
    }

    /// <summary>
    /// Filters for tickers alphabetically within the specified range.
    /// </summary>
    /// <param name="from">The starting ticker value (inclusive).</param>
    /// <param name="to">The ending ticker value (inclusive).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder Between(string from, string to)
    {
        _tickerGte = from;
        _tickerLte = to;
        return this;
    }

    /// <summary>
    /// Filters tickers by security type.
    /// </summary>
    /// <param name="type">The security type (e.g., "CS" for Common Stock, "ETF" for Exchange Traded Fund).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder OfType(string type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Filters tickers by market type.
    /// </summary>
    /// <param name="market">The market type to filter by.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder InMarket(Market market)
    {
        _market = market;
        return this;
    }

    /// <summary>
    /// Filters tickers by primary exchange.
    /// </summary>
    /// <param name="exchange">The exchange code (e.g., "XNYS", "XNAS").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder OnExchange(string exchange)
    {
        _exchange = exchange;
        return this;
    }

    /// <summary>
    /// Filters tickers by CUSIP identifier.
    /// </summary>
    /// <param name="cusip">The CUSIP identifier.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder WithCusip(string cusip)
    {
        _cusip = cusip;
        return this;
    }

    /// <summary>
    /// Filters tickers by SEC Central Index Key (CIK).
    /// </summary>
    /// <param name="cik">The CIK identifier.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder WithCik(string cik)
    {
        _cik = cik;
        return this;
    }

    /// <summary>
    /// Filters for tickers active on the specified date.
    /// </summary>
    /// <param name="date">The date in YYYY-MM-DD format.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder ActiveOn(string date)
    {
        _date = date;
        return this;
    }

    /// <summary>
    /// Searches for tickers by name or ticker symbol.
    /// </summary>
    /// <param name="searchTerm">The search term to match against ticker names and symbols.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder Search(string searchTerm)
    {
        _search = searchTerm;
        return this;
    }

    /// <summary>
    /// Filters for active tickers only.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder ActiveOnly()
    {
        _active = true;
        return this;
    }

    /// <summary>
    /// Filters for inactive tickers only.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder InactiveOnly()
    {
        _active = false;
        return this;
    }

    /// <summary>
    /// Specifies the field to sort results by.
    /// </summary>
    /// <param name="field">The field name to sort by.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder SortBy(string field)
    {
        _sort = field;
        return this;
    }

    /// <summary>
    /// Sets the sort order to ascending.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder Ascending()
    {
        _order = SortOrder.Ascending;
        return this;
    }

    /// <summary>
    /// Sets the sort order to descending.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder Descending()
    {
        _order = SortOrder.Descending;
        return this;
    }

    /// <summary>
    /// Limits the number of results returned.
    /// </summary>
    /// <param name="limit">The maximum number of results to return (maximum 1000).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public TickersQueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the tickers response.</returns>
    public Task<PolygonResponse<List<StockTicker>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = new GetTickersRequest
        {
            Ticker = _ticker,
            TickerGt = _tickerGt,
            TickerGte = _tickerGte,
            TickerLt = _tickerLt,
            TickerLte = _tickerLte,
            Type = _type,
            Market = _market,
            Exchange = _exchange,
            Cusip = _cusip,
            Cik = _cik,
            Date = _date,
            Search = _search,
            Active = _active,
            Sort = _sort,
            Order = _order,
            Limit = _limit
        };

        return _service.GetTickersAsync(request, cancellationToken);
    }
}
