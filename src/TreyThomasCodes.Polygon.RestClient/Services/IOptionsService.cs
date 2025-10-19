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

    // Component-based convenience methods

    /// <summary>
    /// Retrieves detailed information about a specific options contract using its component parts instead of a pre-formatted ticker.
    /// This is a convenience method that constructs the OCC format ticker string from the provided components.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "UBER", "SPY", "AAPL").</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract (e.g., 50 for $50).</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract details.</returns>
    Task<PolygonResponse<OptionsContract>> GetContractByComponentsAsync(
        string underlying,
        DateOnly expirationDate,
        OptionType type,
        decimal strike,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a snapshot of current market data for a specific options contract using its component parts.
    /// This is a convenience method that constructs the ticker from the provided components.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY", "AAPL").</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract snapshot.</returns>
    Task<PolygonResponse<OptionSnapshot>> GetSnapshotByComponentsAsync(
        string underlying,
        DateOnly expirationDate,
        OptionType type,
        decimal strike,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a snapshot of current market data for all options contracts for a given underlying asset using component parameters.
    /// This is a convenience method that constructs the request from strongly-typed parameters instead of requiring a request object.
    /// Returns comprehensive market information for each contract including break-even price, daily data, contract details, Greeks, implied volatility, and open interest.
    /// Supports pagination and filtering by expiration date, strike price, and contract type.
    /// </summary>
    /// <param name="underlyingAsset">The ticker symbol of the underlying asset (e.g., "SPY", "AAPL", "MSTR").</param>
    /// <param name="strikePrice">Optional filter to only return contracts with this exact strike price.</param>
    /// <param name="type">Optional filter for option type (Call or Put). If null, returns both types.</param>
    /// <param name="expirationDateGte">Optional minimum expiration date. Only returns contracts expiring on or after this date.</param>
    /// <param name="expirationDateLte">Optional maximum expiration date. Only returns contracts expiring on or before this date.</param>
    /// <param name="limit">Optional limit on the number of results to return. Maximum value varies by plan.</param>
    /// <param name="order">Optional sort order for results. Use "asc" for ascending or "desc" for descending.</param>
    /// <param name="sort">Optional field to sort by (e.g., "ticker", "strike_price", "expiration_date").</param>
    /// <param name="cursor">Optional cursor for pagination. Use the next_url from the previous response to get the next page of results.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option snapshots for all contracts matching the specified criteria. The response includes a next_url for pagination if more results are available.</returns>
    Task<PolygonResponse<List<OptionSnapshot>>> GetChainSnapshotByComponentsAsync(
        string underlyingAsset,
        decimal? strikePrice = null,
        OptionType? type = null,
        DateOnly? expirationDateGte = null,
        DateOnly? expirationDateLte = null,
        int? limit = null,
        string? order = null,
        string? sort = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent trade for a specific options contract using its component parts.
    /// This is a convenience method that constructs the OCC format ticker string from the provided components.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "TSLA", "SPY").</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the most recent trade data.</returns>
    Task<PolygonResponse<OptionTrade>> GetLastTradeByComponentsAsync(
        string underlying,
        DateOnly expirationDate,
        OptionType type,
        decimal strike,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves aggregate OHLC (bar/candle) data for an options contract using its component parts.
    /// This is a convenience method that constructs the OCC format ticker string from the provided components.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY").</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract.</param>
    /// <param name="multiplier">The number of timespan units to aggregate (e.g., 1 for 1 day, 5 for 5 minutes).</param>
    /// <param name="timespan">The size of the time window for each aggregate (e.g., minute, hour, day).</param>
    /// <param name="from">Start date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="to">End date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="adjusted">Whether to adjust for splits. Defaults to true if not specified.</param>
    /// <param name="sort">Sort order for results (asc for ascending, desc for descending by timestamp).</param>
    /// <param name="limit">Limit the number of aggregate results returned.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of aggregate OHLC bars.</returns>
    Task<PolygonResponse<List<OptionBar>>> GetBarsByComponentsAsync(
        string underlying,
        DateOnly expirationDate,
        OptionType type,
        decimal strike,
        int multiplier,
        AggregateInterval timespan,
        string from,
        string to,
        bool? adjusted = null,
        SortOrder? sort = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    // Discovery helper methods

    /// <summary>
    /// Discovers available strike prices for options on a given underlying asset.
    /// This method queries the options chain and returns a sorted list of unique strike prices.
    /// Optionally filters by expiration date range and option type.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY", "AAPL").</param>
    /// <param name="type">Optional filter for option type (Call or Put). If null, returns strikes for both.</param>
    /// <param name="expirationDateGte">Optional filter for minimum expiration date in YYYY-MM-DD format.</param>
    /// <param name="expirationDateLte">Optional filter for maximum expiration date in YYYY-MM-DD format.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a sorted list of available strike prices.</returns>
    Task<List<decimal>> GetAvailableStrikesAsync(
        string underlying,
        OptionType? type = null,
        string? expirationDateGte = null,
        string? expirationDateLte = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers available expiration dates for options on a given underlying asset.
    /// This method queries the options chain and returns a sorted list of unique expiration dates.
    /// Optionally filters by option type and strike price.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY", "AAPL").</param>
    /// <param name="type">Optional filter for option type (Call or Put). If null, returns dates for both.</param>
    /// <param name="strikePrice">Optional filter for a specific strike price.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a sorted list of available expiration dates.</returns>
    Task<List<DateOnly>> GetExpirationDatesAsync(
        string underlying,
        OptionType? type = null,
        decimal? strikePrice = null,
        CancellationToken cancellationToken = default);

    // OptionsTicker-based overload methods

    /// <summary>
    /// Retrieves detailed information about a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// </summary>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract details.</returns>
    Task<PolygonResponse<OptionsContract>> GetContractDetailsAsync(
        OptionsTicker ticker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a snapshot of current market data for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// </summary>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract snapshot.</returns>
    Task<PolygonResponse<OptionSnapshot>> GetSnapshotAsync(
        OptionsTicker ticker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent trade for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// </summary>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the most recent trade data.</returns>
    Task<PolygonResponse<OptionTrade>> GetLastTradeAsync(
        OptionsTicker ticker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves historical quotes (bid/ask) for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns a list of quotes containing bid and ask prices, sizes, exchange information, and timing data.
    /// Supports pagination and time-based filtering.
    /// </summary>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="timestamp">Query for quotes at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampLt">Query for quotes before this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampLte">Query for quotes at or before this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampGt">Query for quotes after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampGte">Query for quotes at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="order">Sort order for results. Use "asc" for ascending or "desc" for descending by timestamp.</param>
    /// <param name="limit">Limit the number of results returned. Maximum value varies by plan.</param>
    /// <param name="sort">Sort field for results. Defaults to "timestamp".</param>
    /// <param name="cursor">Cursor for pagination. Use the next_url from the previous response to get the next page of results.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option quotes including bid/ask prices, sizes, exchanges, sequence numbers, and timestamps.</returns>
    Task<PolygonResponse<List<OptionQuote>>> GetQuotesAsync(
        OptionsTicker ticker,
        string? timestamp = null,
        string? timestampLt = null,
        string? timestampLte = null,
        string? timestampGt = null,
        string? timestampGte = null,
        string? order = null,
        int? limit = null,
        string? sort = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves historical trade data for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns a list of trades containing price, size, exchange, conditions, and timing data.
    /// Supports pagination and time-based filtering.
    /// </summary>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="timestamp">Query for trades at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampLt">Query for trades before this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampLte">Query for trades at or before this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampGt">Query for trades after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampGte">Query for trades at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="order">Sort order for results. Use "asc" for ascending or "desc" for descending by timestamp.</param>
    /// <param name="limit">Limit the number of results returned. Maximum value varies by plan.</param>
    /// <param name="sort">Sort field for results. Defaults to "timestamp".</param>
    /// <param name="cursor">Cursor for pagination. Use the next_url from the previous response to get the next page of results.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option trades including price, size, exchange, conditions, sequence numbers, and timestamps.</returns>
    Task<PolygonResponse<List<OptionTradeV3>>> GetTradesAsync(
        OptionsTicker ticker,
        string? timestamp = null,
        string? timestampLt = null,
        string? timestampLte = null,
        string? timestampGt = null,
        string? timestampGte = null,
        string? order = null,
        int? limit = null,
        string? sort = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves aggregate OHLC (bar/candle) data for an options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns historical pricing data aggregated by the specified time interval, useful for charting and technical analysis.
    /// </summary>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="multiplier">The number of timespan units to aggregate (e.g., 1 for 1 day, 5 for 5 minutes).</param>
    /// <param name="timespan">The size of the time window for each aggregate (e.g., minute, hour, day).</param>
    /// <param name="from">Start date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="to">End date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="adjusted">Whether to adjust for splits. Defaults to true if not specified.</param>
    /// <param name="sort">Sort order for results (asc for ascending, desc for descending by timestamp).</param>
    /// <param name="limit">Limit the number of aggregate results returned.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of aggregate OHLC bars.</returns>
    Task<PolygonResponse<List<OptionBar>>> GetBarsAsync(
        OptionsTicker ticker,
        int multiplier,
        AggregateInterval timespan,
        string from,
        string to,
        bool? adjusted = null,
        SortOrder? sort = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the daily open, high, low, close (OHLC) summary for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns comprehensive daily trading data including opening and closing prices, high and low prices, trading volume, and pre-market and after-hours prices.
    /// </summary>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="date">The date of the requested daily data in YYYY-MM-DD format (e.g., "2023-01-09").</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the daily OHLC summary.</returns>
    Task<OptionDailyOpenClose> GetDailyOpenCloseAsync(
        OptionsTicker ticker,
        string date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the previous trading day's OHLC data for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns the most recent completed trading session's open, high, low, close, volume, and volume-weighted average price data.
    /// </summary>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="adjusted">Whether to adjust for underlying stock splits. Defaults to true if not specified.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the previous trading day's OHLC bar data.</returns>
    Task<PolygonResponse<List<OptionBar>>> GetPreviousDayBarAsync(
        OptionsTicker ticker,
        bool? adjusted = null,
        CancellationToken cancellationToken = default);
}
