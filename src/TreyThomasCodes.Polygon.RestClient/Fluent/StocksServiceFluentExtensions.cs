// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Stocks;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent;

/// <summary>
/// Fluent API extension methods for the IStocksService.
/// These extensions provide a more expressive and chainable API for building stock data queries.
/// To use these extensions, add "using TreyThomasCodes.Polygon.RestClient.Fluent;" to your file.
/// </summary>
public static class StocksServiceFluentExtensions
{
    /// <summary>
    /// Initiates a fluent query builder for retrieving aggregate bars (OHLC candlestick data) for a stock.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <param name="ticker">Optional ticker symbol to initialize the query with (e.g., "AAPL", "MSFT").</param>
    /// <returns>A fluent query builder for configuring and executing the bars request.</returns>
    public static BarsQueryBuilder Bars(this IStocksService service, string? ticker = null)
    {
        return new BarsQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving trade data for a stock.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <param name="ticker">Optional ticker symbol to initialize the query with (e.g., "AAPL", "MSFT").</param>
    /// <returns>A fluent query builder for configuring and executing the trades request.</returns>
    public static TradesQueryBuilder Trades(this IStocksService service, string? ticker = null)
    {
        return new TradesQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving quote data (bid/ask) for a stock.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <param name="ticker">Optional ticker symbol to initialize the query with (e.g., "AAPL", "MSFT").</param>
    /// <returns>A fluent query builder for configuring and executing the quotes request.</returns>
    public static QuotesQueryBuilder Quotes(this IStocksService service, string? ticker = null)
    {
        return new QuotesQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving a snapshot of current market data for a stock.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <param name="ticker">Optional ticker symbol to initialize the query with (e.g., "AAPL", "MSFT").</param>
    /// <returns>A fluent query builder for configuring and executing the snapshot request.</returns>
    public static SnapshotQueryBuilder Snapshot(this IStocksService service, string? ticker = null)
    {
        return new SnapshotQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving the most recent trade for a stock.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <param name="ticker">Optional ticker symbol to initialize the query with (e.g., "AAPL", "MSFT").</param>
    /// <returns>A fluent query builder for configuring and executing the last trade request.</returns>
    public static LastTradeQueryBuilder LastTrade(this IStocksService service, string? ticker = null)
    {
        return new LastTradeQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving the most recent quote (NBBO) for a stock.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <param name="ticker">Optional ticker symbol to initialize the query with (e.g., "AAPL", "MSFT").</param>
    /// <returns>A fluent query builder for configuring and executing the last quote request.</returns>
    public static LastQuoteQueryBuilder LastQuote(this IStocksService service, string? ticker = null)
    {
        return new LastQuoteQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving the previous day's close data for a stock.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <param name="ticker">Optional ticker symbol to initialize the query with (e.g., "AAPL", "MSFT").</param>
    /// <returns>A fluent query builder for configuring and executing the previous close request.</returns>
    public static PreviousCloseQueryBuilder PreviousClose(this IStocksService service, string? ticker = null)
    {
        return new PreviousCloseQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving grouped daily aggregates for all tickers.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <param name="date">Optional date to initialize the query with (YYYY-MM-DD format).</param>
    /// <returns>A fluent query builder for configuring and executing the grouped daily request.</returns>
    public static GroupedDailyQueryBuilder GroupedDaily(this IStocksService service, string? date = null)
    {
        return new GroupedDailyQueryBuilder(service, date);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving daily open/close data for a stock.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <param name="ticker">Optional ticker symbol to initialize the query with (e.g., "AAPL", "MSFT").</param>
    /// <returns>A fluent query builder for configuring and executing the daily open/close request.</returns>
    public static DailyOpenCloseQueryBuilder DailyOpenClose(this IStocksService service, string? ticker = null)
    {
        return new DailyOpenCloseQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving market snapshot data for all tickers.
    /// </summary>
    /// <param name="service">The stocks service instance.</param>
    /// <returns>A fluent query builder for configuring and executing the market snapshot request.</returns>
    public static MarketSnapshotQueryBuilder MarketSnapshot(this IStocksService service)
    {
        return new MarketSnapshotQueryBuilder(service);
    }
}
