// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Services;

/// <summary>
/// Service interface for accessing Polygon.io stocks market data.
/// Provides methods to retrieve real-time and historical stock trades, quotes, snapshots, and OHLC aggregates.
/// </summary>
public interface IStocksService
{
    /// <summary>
    /// Retrieves aggregate (OHLC) data for a specific ticker within a date range.
    /// Returns historical candlestick data with configurable time intervals.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <param name="multiplier">The size of the timespan multiplier (e.g., 1 for 1 minute, 5 for 5 minutes).</param>
    /// <param name="timespan">The size of the time window. Valid values: minute, hour, day, week, month, quarter, year.</param>
    /// <param name="from">The start date for the aggregate window (YYYY-MM-DD format).</param>
    /// <param name="to">The end date for the aggregate window (YYYY-MM-DD format).</param>
    /// <param name="adjusted">Whether the results should be adjusted for stock splits and dividends.</param>
    /// <param name="sort">Sort order: "asc" (ascending) or "desc" (descending).</param>
    /// <param name="limit">The maximum number of results to return (max 50,000).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of aggregate records.</returns>
    Task<PolygonResponse<List<StockBar>>> GetBarsAsync(
        string ticker,
        int multiplier,
        string timespan,
        string from,
        string to,
        bool? adjusted = null,
        string? sort = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the previous trading day's close price for a specific ticker.
    /// Returns the OHLC data for the most recent completed trading session.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <param name="adjusted">Whether the results should be adjusted for stock splits and dividends.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the previous close data.</returns>
    Task<PolygonResponse<List<StockBar>>> GetPreviousCloseAsync(
        string ticker,
        bool? adjusted = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves grouped daily aggregate data for all tickers on a specific date.
    /// Returns OHLC data for all available stock tickers for the specified trading day.
    /// </summary>
    /// <param name="date">The date to get aggregate data for (YYYY-MM-DD format).</param>
    /// <param name="adjusted">Whether the results should be adjusted for stock splits and dividends.</param>
    /// <param name="includeOtc">Whether to include over-the-counter (OTC) securities in the results.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of daily aggregates for all tickers.</returns>
    Task<PolygonResponse<List<StockBar>>> GetGroupedDailyAsync(
        string date,
        bool? adjusted = null,
        bool? includeOtc = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the daily open and close prices for a specific ticker on a specific date.
    /// Returns detailed daily price information including pre-market and after-hours activity.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <param name="date">The date to get open/close data for (YYYY-MM-DD format).</param>
    /// <param name="adjusted">Whether the results should be adjusted for stock splits and dividends.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the daily open/close data.</returns>
    Task<PolygonResponse<List<StockBar>>> GetDailyOpenCloseAsync(
        string ticker,
        string date,
        bool? adjusted = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves trade data for a specific stock ticker from Polygon.io.
    /// Returns tick-level trade data including price, size, exchange, and timestamp information.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <param name="timestamp">Return results for this specific timestamp (nanoseconds since Unix epoch).</param>
    /// <param name="timestampGte">Return results where timestamp is greater than or equal to this value.</param>
    /// <param name="timestampGt">Return results where timestamp is greater than this value.</param>
    /// <param name="timestampLte">Return results where timestamp is less than or equal to this value.</param>
    /// <param name="timestampLt">Return results where timestamp is less than this value.</param>
    /// <param name="limit">The maximum number of results to return (max 50,000).</param>
    /// <param name="sort">Sort order for results. Either "timestamp" (default) or "timestamp.desc".</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of trade records.</returns>
    Task<PolygonResponse<List<StockTrade>>> GetTradesAsync(
        string ticker,
        string? timestamp = null,
        string? timestampGte = null,
        string? timestampGt = null,
        string? timestampLte = null,
        string? timestampLt = null,
        int? limit = null,
        string? sort = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves quote data for a specific stock ticker from Polygon.io.
    /// Returns tick-level quote data including bid/ask prices, sizes, and exchange information.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <param name="timestamp">Return results for this specific timestamp (nanoseconds since Unix epoch).</param>
    /// <param name="timestampGte">Return results where timestamp is greater than or equal to this value.</param>
    /// <param name="timestampGt">Return results where timestamp is greater than this value.</param>
    /// <param name="timestampLte">Return results where timestamp is less than or equal to this value.</param>
    /// <param name="timestampLt">Return results where timestamp is less than this value.</param>
    /// <param name="limit">The maximum number of results to return (max 50,000).</param>
    /// <param name="sort">Sort order for results. Either "timestamp" (default) or "timestamp.desc".</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of quote records.</returns>
    Task<PolygonResponse<List<StockQuote>>> GetQuotesAsync(
        string ticker,
        string? timestamp = null,
        string? timestampGte = null,
        string? timestampGt = null,
        string? timestampLte = null,
        string? timestampLt = null,
        int? limit = null,
        string? sort = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves snapshot data for all available stock tickers.
    /// Returns current market data including last trade, quote, and daily aggregate information.
    /// </summary>
    /// <param name="includeOtc">Whether to include over-the-counter (OTC) securities in the results.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of ticker snapshots.</returns>
    Task<PolygonResponse<List<StockSnapshot>>> GetMarketSnapshotAsync(
        bool? includeOtc = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves snapshot data for a specific stock ticker.
    /// Returns current market data including last trade, quote, and daily aggregate information.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the ticker snapshot.</returns>
    Task<StockSnapshotResponse> GetSnapshotAsync(
        string ticker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent trade for a specific stock ticker.
    /// Returns detailed information about the last executed trade including price, size, exchange, and timing data.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the most recent trade data.</returns>
    Task<PolygonResponse<StockTrade>> GetLastTradeAsync(
        string ticker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent National Best Bid and Offer (NBBO) quote for a specific stock ticker.
    /// Returns the current best bid and ask prices along with exchange and timing information.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the most recent NBBO quote data.</returns>
    Task<PolygonResponse<LastQuoteResult>> GetLastQuoteAsync(
        string ticker,
        CancellationToken cancellationToken = default);
}