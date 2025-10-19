// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

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
    /// <param name="request">The request containing ticker, date range, and aggregation parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of aggregate records.</returns>
    Task<PolygonResponse<List<StockBar>>> GetBarsAsync(
        GetBarsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the previous trading day's close price for a specific ticker.
    /// Returns the OHLC data for the most recent completed trading session.
    /// </summary>
    /// <param name="request">The request containing the ticker and adjustment parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the previous close data.</returns>
    Task<PolygonResponse<List<StockBar>>> GetPreviousCloseAsync(
        GetPreviousCloseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves grouped daily aggregate data for all tickers on a specific date.
    /// Returns OHLC data for all available stock tickers for the specified trading day.
    /// </summary>
    /// <param name="request">The request containing the date and filtering parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of daily aggregates for all tickers.</returns>
    Task<PolygonResponse<List<StockBar>>> GetGroupedDailyAsync(
        GetGroupedDailyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the daily open and close prices for a specific ticker on a specific date.
    /// Returns detailed daily price information including pre-market and after-hours activity.
    /// </summary>
    /// <param name="request">The request containing the ticker, date, and adjustment parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the daily open/close data.</returns>
    Task<PolygonResponse<List<StockBar>>> GetDailyOpenCloseAsync(
        GetDailyOpenCloseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves trade data for a specific stock ticker from Polygon.io.
    /// Returns tick-level trade data including price, size, exchange, and timestamp information.
    /// </summary>
    /// <param name="request">The request containing the ticker and filtering parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of trade records.</returns>
    Task<PolygonResponse<List<StockTrade>>> GetTradesAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves quote data for a specific stock ticker from Polygon.io.
    /// Returns tick-level quote data including bid/ask prices, sizes, and exchange information.
    /// </summary>
    /// <param name="request">The request containing the ticker and filtering parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of quote records.</returns>
    Task<PolygonResponse<List<StockQuote>>> GetQuotesAsync(
        GetQuotesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves snapshot data for all available stock tickers.
    /// Returns current market data including last trade, quote, and daily aggregate information.
    /// </summary>
    /// <param name="request">The request containing filtering parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of ticker snapshots.</returns>
    Task<PolygonResponse<List<StockSnapshot>>> GetMarketSnapshotAsync(
        GetMarketSnapshotRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves snapshot data for a specific stock ticker.
    /// Returns current market data including last trade, quote, and daily aggregate information.
    /// </summary>
    /// <param name="request">The request containing the ticker symbol.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the ticker snapshot.</returns>
    Task<StockSnapshotResponse> GetSnapshotAsync(
        GetSnapshotRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves snapshot data for a specific stock ticker.
    /// Returns current market data including last trade, quote, and daily aggregate information.
    /// This is a convenience overload that accepts the ticker symbol directly.
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
    /// <param name="request">The request containing the ticker symbol.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the most recent trade data.</returns>
    Task<PolygonResponse<StockTrade>> GetLastTradeAsync(
        GetLastTradeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent trade for a specific stock ticker.
    /// Returns detailed information about the last executed trade including price, size, exchange, and timing data.
    /// This is a convenience overload that accepts the ticker symbol directly.
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
    /// <param name="request">The request containing the ticker symbol.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the most recent NBBO quote data.</returns>
    Task<PolygonResponse<LastQuoteResult>> GetLastQuoteAsync(
        GetLastQuoteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent National Best Bid and Offer (NBBO) quote for a specific stock ticker.
    /// Returns the current best bid and ask prices along with exchange and timing information.
    /// This is a convenience overload that accepts the ticker symbol directly.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL", "MSFT").</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the most recent NBBO quote data.</returns>
    Task<PolygonResponse<LastQuoteResult>> GetLastQuoteAsync(
        string ticker,
        CancellationToken cancellationToken = default);
}