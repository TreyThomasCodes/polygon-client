// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Refit;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;

namespace TreyThomasCodes.Polygon.RestClient.Api;

/// <summary>
/// Provides access to Polygon.io Options API endpoints for retrieving options contract data including trades, quotes, and market snapshots.
/// This interface serves as a foundation for future options-related endpoint implementations.
/// </summary>
public interface IPolygonOptionsApi
{
    /// <summary>
    /// Retrieves detailed information about a specific options contract by its ticker symbol.
    /// Returns comprehensive contract specifications including strike price, expiration date, contract type, exercise style, and underlying asset information.
    /// </summary>
    /// <param name="optionsTicker">The options ticker symbol in OCC format (e.g., "O:SPY251219C00650000"). The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract details including CFI code, contract type, exercise style, expiration date, primary exchange, shares per contract, strike price, ticker, and underlying ticker.</returns>
    [Get("/v3/reference/options/contracts/{optionsTicker}")]
    Task<PolygonResponse<OptionsContract>> GetContractDetailsAsync(
        string optionsTicker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a snapshot of current market data for a specific options contract.
    /// Returns comprehensive market information including the most recent trade, quote, daily aggregate data, Greeks, implied volatility, open interest, and underlying asset details.
    /// </summary>
    /// <param name="underlyingAsset">The ticker symbol of the underlying asset (e.g., "SPY", "AAPL").</param>
    /// <param name="optionContract">The options contract identifier in OCC format without the "O:" prefix (e.g., "SPY251219C00650000").</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract snapshot including break-even price, daily data, contract details, Greeks, implied volatility, last quote, last trade, open interest, and underlying asset information.</returns>
    [Get("/v3/snapshot/options/{underlyingAsset}/{optionContract}")]
    Task<PolygonResponse<OptionSnapshot>> GetSnapshotAsync(
        string underlyingAsset,
        string optionContract,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a snapshot of current market data for all options contracts for a given underlying asset.
    /// Returns a collection of option snapshots containing comprehensive market information for each contract including break-even price, daily data, contract details, Greeks, implied volatility, open interest, and underlying asset details.
    /// Supports pagination and filtering by expiration date, strike price, and contract type.
    /// </summary>
    /// <param name="underlyingAsset">The ticker symbol of the underlying asset (e.g., "SPY", "AAPL", "MSTR").</param>
    /// <param name="strikePrice">Filter by strike price. Only returns contracts with this exact strike price.</param>
    /// <param name="contractType">Filter by contract type. Use "call" for call options or "put" for put options.</param>
    /// <param name="expirationDateGte">Filter by expiration date greater than or equal to the specified date in YYYY-MM-DD format.</param>
    /// <param name="expirationDateLte">Filter by expiration date less than or equal to the specified date in YYYY-MM-DD format.</param>
    /// <param name="limit">Limit the number of results returned. Maximum value varies by plan.</param>
    /// <param name="order">Sort order for results. Use "asc" for ascending or "desc" for descending.</param>
    /// <param name="sort">Field to sort by (e.g., "ticker", "strike_price", "expiration_date").</param>
    /// <param name="cursor">Cursor for pagination. Use the next_url from the previous response to get the next page of results.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option snapshots for all contracts matching the specified criteria. The response includes a next_url for pagination if more results are available.</returns>
    [Get("/v3/snapshot/options/{underlyingAsset}")]
    Task<PolygonResponse<List<OptionSnapshot>>> GetChainSnapshotAsync(
        string underlyingAsset,
        [Query][AliasAs("strike_price")] decimal? strikePrice = null,
        [Query][AliasAs("contract_type")] string? contractType = null,
        [Query][AliasAs("expiration_date.gte")] string? expirationDateGte = null,
        [Query][AliasAs("expiration_date.lte")] string? expirationDateLte = null,
        [Query] int? limit = null,
        [Query] string? order = null,
        [Query] string? sort = null,
        [Query] string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent trade for a specific options contract.
    /// Returns detailed information about the last executed trade including price, size, exchange, conditions, and timing data.
    /// </summary>
    /// <param name="optionsTicker">The options ticker symbol in OCC format (e.g., "O:TSLA260320C00700000"). The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the most recent trade data for the specified options contract including ticker, price, size, exchange, conditions, sequence number, and timestamp.</returns>
    [Get("/v2/last/trade/{optionsTicker}")]
    Task<PolygonResponse<OptionTrade>> GetLastTradeAsync(
        string optionsTicker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves historical quotes (bid/ask) for a specific options contract.
    /// Returns a list of quotes containing bid and ask prices, sizes, exchange information, and timing data.
    /// Supports pagination and time-based filtering.
    /// </summary>
    /// <param name="optionsTicker">The options ticker symbol in OCC format (e.g., "O:SPY241220P00720000"). The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.</param>
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
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option quotes including bid/ask prices, sizes, exchanges, sequence numbers, and timestamps. The response includes a next_url for pagination if more results are available.</returns>
    [Get("/v3/quotes/{optionsTicker}")]
    Task<PolygonResponse<List<OptionQuote>>> GetQuotesAsync(
        string optionsTicker,
        [Query] string? timestamp = null,
        [Query][AliasAs("timestamp.lt")] string? timestampLt = null,
        [Query][AliasAs("timestamp.lte")] string? timestampLte = null,
        [Query][AliasAs("timestamp.gt")] string? timestampGt = null,
        [Query][AliasAs("timestamp.gte")] string? timestampGte = null,
        [Query] string? order = null,
        [Query] int? limit = null,
        [Query] string? sort = null,
        [Query] string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves historical trade data for a specific options contract.
    /// Returns a list of trades containing price, size, exchange, conditions, and timing data.
    /// Supports pagination and time-based filtering.
    /// </summary>
    /// <param name="optionsTicker">The options ticker symbol in OCC format (e.g., "O:TSLA210903C00700000"). The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.</param>
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
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option trades including price, size, exchange, conditions, sequence numbers, and timestamps. The response includes a next_url for pagination if more results are available.</returns>
    [Get("/v3/trades/{optionsTicker}")]
    Task<PolygonResponse<List<OptionTradeV3>>> GetTradesAsync(
        string optionsTicker,
        [Query] string? timestamp = null,
        [Query][AliasAs("timestamp.lt")] string? timestampLt = null,
        [Query][AliasAs("timestamp.lte")] string? timestampLte = null,
        [Query][AliasAs("timestamp.gt")] string? timestampGt = null,
        [Query][AliasAs("timestamp.gte")] string? timestampGte = null,
        [Query] string? order = null,
        [Query] int? limit = null,
        [Query] string? sort = null,
        [Query] string? cursor = null,
        CancellationToken cancellationToken = default);
}
