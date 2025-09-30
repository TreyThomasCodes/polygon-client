// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Refit;
using TTC.Polygon.Models.Common;
using TTC.Polygon.Models.Stocks;

namespace TTC.Polygon.RestClient.Api;

/// <summary>
/// Provides access to Polygon.io Stocks API endpoints for retrieving stock market data including trades, quotes, and market snapshots.
/// </summary>
public interface IPolygonStocksApi
{
    /// <summary>
    /// Retrieves aggregate OHLC data for a stock ticker over a specified time range. This endpoint provides candlestick data useful for charting and technical analysis.
    /// </summary>
    /// <param name="ticker">The ticker symbol for which to retrieve aggregate data (e.g., "AAPL", "MSFT").</param>
    /// <param name="multiplier">The number of timespan units to aggregate (e.g., 1 for 1 day, 5 for 5 minutes).</param>
    /// <param name="timespan">The size of the time window for each aggregate (e.g., "minute", "hour", "day", "week", "month", "quarter", "year").</param>
    /// <param name="from">Start date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="to">End date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="adjusted">Whether to adjust for stock splits and dividend payments. Defaults to true if not specified.</param>
    /// <param name="sort">Sort order for results ("asc" for ascending or "desc" for descending by timestamp).</param>
    /// <param name="limit">Limit the number of aggregate results returned (maximum 50,000).</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of aggregate OHLC data points.</returns>
    [Get("/v2/aggs/ticker/{ticker}/range/{multiplier}/{timespan}/{from}/{to}")]
    Task<PolygonResponse<List<StockBar>>> GetBarsAsync(
        string ticker,
        int multiplier,
        string timespan,
        string from,
        string to,
        [Query] bool? adjusted = null,
        [Query] string? sort = null,
        [Query] int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the previous trading day's OHLC data for a specific stock ticker. This is useful for calculating daily price changes and percentage movements.
    /// </summary>
    /// <param name="ticker">The ticker symbol for which to retrieve previous close data (e.g., "AAPL", "MSFT").</param>
    /// <param name="adjusted">Whether to adjust for stock splits and dividend payments. Defaults to true if not specified.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the previous trading day's OHLC data.</returns>
    [Get("/v2/aggs/ticker/{ticker}/prev")]
    Task<PolygonResponse<List<StockBar>>> GetPreviousCloseAsync(
        string ticker,
        [Query] bool? adjusted = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the daily OHLC aggregate data for all US stock tickers on a specific date. This endpoint provides a comprehensive market overview for a given trading day.
    /// </summary>
    /// <param name="date">The date for which to retrieve grouped daily aggregates in YYYY-MM-DD format.</param>
    /// <param name="adjusted">Whether to adjust for stock splits and dividend payments. Defaults to true if not specified.</param>
    /// <param name="includeOtc">Whether to include over-the-counter (OTC) securities in the results. Defaults to false if not specified.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of daily OHLC aggregates for all tickers.</returns>
    [Get("/v2/aggs/grouped/locale/us/market/stocks/{date}")]
    Task<PolygonResponse<List<StockBar>>> GetGroupedDailyAsync(
        string date,
        [Query] bool? adjusted = null,
        [Query][AliasAs("include_otc")] bool? includeOtc = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the daily open and close prices for a specific stock ticker on a given date. Includes additional data such as high, low, volume, and after-hours trading information.
    /// </summary>
    /// <param name="ticker">The ticker symbol for which to retrieve daily open/close data (e.g., "AAPL", "MSFT").</param>
    /// <param name="date">The date for which to retrieve open/close data in YYYY-MM-DD format.</param>
    /// <param name="adjusted">Whether to adjust for stock splits and dividend payments. Defaults to true if not specified.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the daily OHLC data for the specified ticker and date.</returns>
    [Get("/v1/open-close/{ticker}/{date}")]
    Task<PolygonResponse<List<StockBar>>> GetDailyOpenCloseAsync(
        string ticker,
        string date,
        [Query] bool? adjusted = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves tick-level trade data for a specific stock ticker symbol. Returns detailed information about individual trades including price, size, and exchange data.
    /// </summary>
    /// <param name="ticker">The ticker symbol for which to retrieve trade data (e.g., "AAPL", "MSFT").</param>
    /// <param name="timestamp">Exact timestamp for querying specific trades (nanosecond precision).</param>
    /// <param name="timestampGte">Filter trades with timestamps greater than or equal to this value.</param>
    /// <param name="timestampGt">Filter trades with timestamps greater than this value.</param>
    /// <param name="timestampLte">Filter trades with timestamps less than or equal to this value.</param>
    /// <param name="timestampLt">Filter trades with timestamps less than this value.</param>
    /// <param name="limit">Limit the number of trades returned (maximum varies by plan).</param>
    /// <param name="sort">Sort order for results (e.g., "timestamp", "timestamp.asc", "timestamp.desc").</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of trade records.</returns>
    [Get("/v3/trades/{ticker}")]
    Task<PolygonResponse<List<StockTrade>>> GetTradesAsync(
        string ticker,
        [Query] string? timestamp = null,
        [Query][AliasAs("timestamp.gte")] string? timestampGte = null,
        [Query][AliasAs("timestamp.gt")] string? timestampGt = null,
        [Query][AliasAs("timestamp.lte")] string? timestampLte = null,
        [Query][AliasAs("timestamp.lt")] string? timestampLt = null,
        [Query] int? limit = null,
        [Query] string? sort = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves top-of-book quote data for a specific stock ticker symbol. Returns bid and ask prices along with size information from various exchanges.
    /// </summary>
    /// <param name="ticker">The ticker symbol for which to retrieve quote data (e.g., "AAPL", "MSFT").</param>
    /// <param name="timestamp">Exact timestamp for querying specific quotes (nanosecond precision).</param>
    /// <param name="timestampGte">Filter quotes with timestamps greater than or equal to this value.</param>
    /// <param name="timestampGt">Filter quotes with timestamps greater than this value.</param>
    /// <param name="timestampLte">Filter quotes with timestamps less than or equal to this value.</param>
    /// <param name="timestampLt">Filter quotes with timestamps less than this value.</param>
    /// <param name="limit">Limit the number of quotes returned (maximum varies by plan).</param>
    /// <param name="sort">Sort order for results (e.g., "timestamp", "timestamp.asc", "timestamp.desc").</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of quote records.</returns>
    [Get("/v3/quotes/{ticker}")]
    Task<PolygonResponse<List<StockQuote>>> GetQuotesAsync(
        string ticker,
        [Query] string? timestamp = null,
        [Query][AliasAs("timestamp.gte")] string? timestampGte = null,
        [Query][AliasAs("timestamp.gt")] string? timestampGt = null,
        [Query][AliasAs("timestamp.lte")] string? timestampLte = null,
        [Query][AliasAs("timestamp.lt")] string? timestampLt = null,
        [Query] int? limit = null,
        [Query] string? sort = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves current market snapshot data for all US stock tickers. Provides a comprehensive overview of the current state of all stocks including last trade, quote, and daily open/high/low/close data.
    /// </summary>
    /// <param name="includeOtc">Whether to include over-the-counter (OTC) securities in the results. Defaults to false if not specified.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of market snapshots for all tickers.</returns>
    [Get("/v2/snapshot/locale/us/markets/stocks/tickers")]
    Task<PolygonResponse<List<StockSnapshot>>> GetMarketSnapshotAsync(
        [Query][AliasAs("include_otc")] bool? includeOtc = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves current market snapshot data for a specific stock ticker. Provides real-time information including the last trade, current bid/ask, and daily OHLC (Open, High, Low, Close) data.
    /// </summary>
    /// <param name="ticker">The ticker symbol for which to retrieve snapshot data (e.g., "AAPL", "MSFT").</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the market snapshot for the specified ticker.</returns>
    [Get("/v2/snapshot/locale/us/markets/stocks/tickers/{ticker}")]
    Task<StockSnapshotResponse> GetSnapshotAsync(
        string ticker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent trade for a specific stock ticker. Returns detailed information about the last executed trade including price, size, exchange, and timing data.
    /// </summary>
    /// <param name="ticker">The ticker symbol for which to retrieve the last trade (e.g., "AAPL", "MSFT").</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the most recent trade data for the specified ticker.</returns>
    [Get("/v2/last/trade/{ticker}")]
    Task<PolygonResponse<StockTrade>> GetLastTradeAsync(
        string ticker,
        CancellationToken cancellationToken = default);
}