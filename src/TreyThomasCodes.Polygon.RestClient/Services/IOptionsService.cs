// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.RestClient.Services;

/// <summary>
/// Service interface for accessing Polygon.io options market data.
/// Provides methods to retrieve options contract information, trades, quotes, snapshots, and OHLC aggregates.
/// This interface serves as a foundation for future options-related method implementations.
/// </summary>
public interface IOptionsService
{
    /// <summary>
    /// Retrieves detailed information about a specific options contract by its ticker symbol.
    /// Returns comprehensive contract specifications including strike price, expiration date, contract type, exercise style, and underlying asset information.
    /// </summary>
    /// <param name="request">The request containing the options ticker in OCC format (e.g., "O:SPY251219C00650000").</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract details including CFI code, contract type, exercise style, expiration date, primary exchange, shares per contract, strike price, ticker, and underlying ticker.</returns>
    Task<PolygonResponse<OptionsContract>> GetContractDetailsAsync(
        GetContractDetailsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a snapshot of current market data for a specific options contract.
    /// Returns comprehensive market information including the most recent trade, quote, daily aggregate data, Greeks, implied volatility, open interest, and underlying asset details.
    /// </summary>
    /// <param name="request">The request containing the underlying asset ticker and options contract identifier.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract snapshot including break-even price, daily data, contract details, Greeks, implied volatility, last quote, last trade, open interest, and underlying asset information.</returns>
    Task<PolygonResponse<OptionSnapshot>> GetSnapshotAsync(
        GetSnapshotRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a snapshot of current market data for all options contracts for a given underlying asset.
    /// Returns a collection of option snapshots containing comprehensive market information for each contract including break-even price, daily data, contract details, Greeks, implied volatility, open interest, and underlying asset details.
    /// Supports pagination and filtering by expiration date, strike price, and contract type.
    /// </summary>
    /// <param name="request">The request containing the underlying asset ticker and optional filters for strike price, contract type, expiration dates, pagination, and sorting.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option snapshots for all contracts matching the specified criteria. The response includes a next_url for pagination if more results are available.</returns>
    Task<PolygonResponse<List<OptionSnapshot>>> GetChainSnapshotAsync(
        GetChainSnapshotRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent trade for a specific options contract.
    /// Returns detailed information about the last executed trade including price, size, exchange, conditions, and timing data.
    /// </summary>
    /// <param name="request">The request containing the options ticker in OCC format (e.g., "O:TSLA260320C00700000").</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the most recent trade data for the specified options contract including ticker, price, size, exchange, conditions, sequence number, and timestamp.</returns>
    Task<PolygonResponse<OptionTrade>> GetLastTradeAsync(
        GetLastTradeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves historical quotes (bid/ask) for a specific options contract.
    /// Returns a list of quotes containing bid and ask prices, sizes, exchange information, and timing data.
    /// Supports pagination and time-based filtering.
    /// </summary>
    /// <param name="request">The request containing the options ticker and optional filters for timestamp ranges, pagination, and sorting.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option quotes including bid/ask prices, sizes, exchanges, sequence numbers, and timestamps. The response includes a next_url for pagination if more results are available.</returns>
    Task<PolygonResponse<List<OptionQuote>>> GetQuotesAsync(
        GetQuotesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves historical trade data for a specific options contract.
    /// Returns a list of trades containing price, size, exchange, conditions, and timing data.
    /// Supports pagination and time-based filtering.
    /// </summary>
    /// <param name="request">The request containing the options ticker and optional filters for timestamp ranges, pagination, and sorting.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option trades including price, size, exchange, conditions, sequence numbers, and timestamps. The response includes a next_url for pagination if more results are available.</returns>
    Task<PolygonResponse<List<OptionTradeV3>>> GetTradesAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves aggregate OHLC (bar/candle) data for an options contract over a specified time range.
    /// Returns historical pricing data aggregated by the specified time interval, useful for charting and technical analysis.
    /// Each bar contains open, high, low, close, volume, and volume-weighted average price for the specified time period.
    /// </summary>
    /// <param name="request">The request containing the options ticker, aggregation parameters (multiplier, timespan), date range (from, to), and optional parameters for adjustment, sort order, and limit.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of aggregate OHLC bars for the specified options contract, including ticker symbol, query metadata, and whether results are adjusted.</returns>
    Task<PolygonResponse<List<OptionBar>>> GetBarsAsync(
        GetBarsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the daily open, high, low, close (OHLC) summary for a specific options contract on a given date.
    /// Returns comprehensive daily trading data including opening and closing prices, high and low prices, trading volume, and pre-market and after-hours prices.
    /// </summary>
    /// <param name="request">The request containing the options ticker and the date for which to retrieve daily data.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the daily OHLC summary including status, date, symbol, open, high, low, close prices, trading volume, pre-market price, and after-hours price for the specified options contract and date.</returns>
    Task<OptionDailyOpenClose> GetDailyOpenCloseAsync(
        GetDailyOpenCloseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the previous trading day's OHLC data for a specific options contract.
    /// Returns the most recent completed trading session's open, high, low, close, volume, and volume-weighted average price data.
    /// This is useful for calculating daily price changes and percentage movements for options contracts.
    /// </summary>
    /// <param name="request">The request containing the options ticker and optional adjustment parameter.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the previous trading day's OHLC bar data including open, high, low, close prices, trading volume, volume-weighted average price, timestamp, and number of transactions.</returns>
    Task<PolygonResponse<List<OptionBar>>> GetPreviousDayBarAsync(
        GetPreviousDayBarRequest request,
        CancellationToken cancellationToken = default);
}
