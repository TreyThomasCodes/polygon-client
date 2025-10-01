// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Refit;
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.Models.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Api;

/// <summary>
/// Provides access to Polygon.io Reference API endpoints for retrieving market metadata and reference information.
/// Includes market status, ticker information, and other reference data endpoints.
/// </summary>
public interface IPolygonReferenceApi
{
    /// <summary>
    /// Retrieves a comprehensive list of ticker symbols supported by Polygon.io. This endpoint provides detailed company information and supports extensive filtering and searching capabilities.
    /// </summary>
    /// <param name="ticker">Filter by exact ticker symbol (e.g., "AAPL").</param>
    /// <param name="tickerGt">Filter tickers alphabetically greater than this value.</param>
    /// <param name="tickerGte">Filter tickers alphabetically greater than or equal to this value.</param>
    /// <param name="tickerLt">Filter tickers alphabetically less than this value.</param>
    /// <param name="tickerLte">Filter tickers alphabetically less than or equal to this value.</param>
    /// <param name="type">Filter by security type (e.g., "CS" for Common Stock, "ETF" for Exchange Traded Fund).</param>
    /// <param name="market">Filter by market type.</param>
    /// <param name="exchange">Filter by primary exchange (e.g., "XNYS", "XNAS").</param>
    /// <param name="cusip">Filter by CUSIP identifier.</param>
    /// <param name="cik">Filter by SEC Central Index Key (CIK).</param>
    /// <param name="date">Filter tickers active on this date in YYYY-MM-DD format.</param>
    /// <param name="search">Search for tickers by name or ticker symbol.</param>
    /// <param name="active">Filter by active status (true for active tickers, false for inactive).</param>
    /// <param name="sort">Field to sort by. Use constants from <see cref="TickerSortFields"/> for valid values.</param>
    /// <param name="order">Sort order.</param>
    /// <param name="limit">Limit the number of results returned (maximum 1000).</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of ticker information.</returns>
    [Get("/v3/reference/tickers")]
    Task<PolygonResponse<List<StockTicker>>> GetTickersAsync(
        [Query] string? ticker = null,
        [Query][AliasAs("ticker.gt")] string? tickerGt = null,
        [Query][AliasAs("ticker.gte")] string? tickerGte = null,
        [Query][AliasAs("ticker.lt")] string? tickerLt = null,
        [Query][AliasAs("ticker.lte")] string? tickerLte = null,
        [Query] string? type = null,
        [Query] Market? market = null,
        [Query] string? exchange = null,
        [Query] string? cusip = null,
        [Query] string? cik = null,
        [Query] string? date = null,
        [Query] string? search = null,
        [Query] bool? active = null,
        [Query] string? sort = null,
        [Query] SortOrder? order = null,
        [Query] int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information for a specific ticker symbol including company name, market classification, locale, and other metadata.
    /// </summary>
    /// <param name="ticker">The ticker symbol for which to retrieve detailed information (e.g., "AAPL", "MSFT").</param>
    /// <param name="date">Get ticker details as of this date in YYYY-MM-DD format. Defaults to the most recent available data if not specified.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with detailed ticker information.</returns>
    [Get("/v3/reference/tickers/{ticker}")]
    Task<PolygonResponse<StockTicker>> GetTickerDetailsAsync(
        string ticker,
        [Query] string? date = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current trading status for various exchanges and overall financial markets.
    /// Returns real-time information about market hours, pre-market and after-hours sessions,
    /// and the status of individual exchanges and index groups.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the current market status information.</returns>
    [Get("/v1/marketstatus/now")]
    Task<MarketStatus> GetMarketStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all ticker types supported by Polygon.io.
    /// Returns information about different security types including their codes, descriptions, asset classes, and locales.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of ticker types.</returns>
    [Get("/v3/reference/tickers/types")]
    Task<TickerTypesResponse> GetTickerTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a unified list of trade and quote conditions from various market data providers.
    /// These conditions provide context for market data events and affect calculations of trading metrics like high, low, open, close, and volume.
    /// </summary>
    /// <param name="assetClass">Filter conditions by asset class.</param>
    /// <param name="dataType">Filter conditions by data type.</param>
    /// <param name="id">Filter by condition ID. Can be a single ID or comma-separated list of IDs.</param>
    /// <param name="sipMapping">Filter by SIP mapping type.</param>
    /// <param name="order">Sort order for the results.</param>
    /// <param name="limit">Limit the number of results returned. Maximum value is 1000. Default is 10.</param>
    /// <param name="sort">Field to sort by. Use constants from <see cref="ConditionCodeSortFields"/> for valid values. Default is "asset_class".</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of condition codes.</returns>
    [Get("/v3/reference/conditions")]
    Task<PolygonResponse<List<ConditionCode>>> GetConditionCodesAsync(
        [Query][AliasAs("asset_class")] AssetClass? assetClass = null,
        [Query][AliasAs("data_type")] DataType? dataType = null,
        [Query] string? id = null,
        [Query][AliasAs("sip_mapping")] SipMappingType? sipMapping = null,
        [Query] SortOrder? order = null,
        [Query] int? limit = null,
        [Query] string? sort = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a comprehensive list of exchanges and market centers from Polygon.io.
    /// Returns information about trading venues including exchanges, trade reporting facilities (TRFs),
    /// securities information processors (SIPs), and other market-related entities.
    /// </summary>
    /// <param name="assetClass">Filter exchanges by asset class.</param>
    /// <param name="locale">Filter exchanges by locale.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of exchanges.</returns>
    [Get("/v3/reference/exchanges")]
    Task<PolygonResponse<List<Exchange>>> GetExchangesAsync(
        [Query][AliasAs("asset_class")] AssetClass? assetClass = null,
        [Query] Locale? locale = null,
        CancellationToken cancellationToken = default);
}
