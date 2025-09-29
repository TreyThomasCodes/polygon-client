// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0


using TTC.Polygon.Models.Common;
using TTC.Polygon.Models.Reference;
using TTC.Polygon.Models.Stocks;

namespace TTC.Polygon.RestClient.Services;

/// <summary>
/// Service interface for accessing Polygon.io reference data and market status information.
/// Provides methods to retrieve ticker information, market metadata, trading status, and other reference information.
/// </summary>
public interface IReferenceDataService
{
    /// <summary>
    /// Retrieves a list of tickers based on various filter criteria.
    /// Returns comprehensive ticker information including metadata and status.
    /// </summary>
    /// <param name="ticker">Return tickers with this exact symbol.</param>
    /// <param name="tickerGt">Return tickers with symbols greater than this value.</param>
    /// <param name="tickerGte">Return tickers with symbols greater than or equal to this value.</param>
    /// <param name="tickerLt">Return tickers with symbols less than this value.</param>
    /// <param name="tickerLte">Return tickers with symbols less than or equal to this value.</param>
    /// <param name="type">Filter by security type (e.g., "CS" for common stock, "ETF" for exchange-traded fund).</param>
    /// <param name="market">Filter by market (e.g., "stocks", "crypto", "forex").</param>
    /// <param name="exchange">Filter by primary exchange (e.g., "NASDAQ", "NYSE").</param>
    /// <param name="cusip">Filter by CUSIP identifier.</param>
    /// <param name="cik">Filter by Central Index Key (CIK) identifier.</param>
    /// <param name="date">Filter by the date the ticker was valid on (YYYY-MM-DD format).</param>
    /// <param name="search">Search for tickers by name or description.</param>
    /// <param name="active">Filter by whether the ticker is actively traded.</param>
    /// <param name="sort">Sort field (e.g., "ticker", "name", "market").</param>
    /// <param name="order">Sort order: "asc" (ascending) or "desc" (descending).</param>
    /// <param name="limit">The maximum number of results to return (max 1,000).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of ticker records.</returns>
    Task<PolygonResponse<List<StockTicker>>> GetTickersAsync(
        string? ticker = null,
        string? tickerGt = null,
        string? tickerGte = null,
        string? tickerLt = null,
        string? tickerLte = null,
        string? type = null,
        string? market = null,
        string? exchange = null,
        string? cusip = null,
        string? cik = null,
        string? date = null,
        string? search = null,
        bool? active = null,
        string? sort = null,
        string? order = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information for a specific ticker symbol.
    /// Returns comprehensive ticker metadata including company information, exchange details, and identifiers.
    /// </summary>
    /// <param name="ticker">The ticker symbol to get details for (e.g., "AAPL", "MSFT").</param>
    /// <param name="date">Get ticker details as of this date (YYYY-MM-DD format). If not provided, returns current details.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing detailed ticker information.</returns>
    Task<PolygonResponse<StockTicker>> GetTickerDetailsAsync(
        string ticker,
        string? date = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current trading status for various exchanges and overall financial markets.
    /// Returns real-time information about market hours, pre-market and after-hours sessions,
    /// and the status of individual exchanges and index groups.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the current market status information.</returns>
    Task<MarketStatus> GetMarketStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all ticker types supported by Polygon.io.
    /// Returns information about different security types including their codes, descriptions, asset classes, and locales.
    /// Examples include "CS" for Common Stock, "ETF" for Exchange Traded Fund, "PFD" for Preferred Stock.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a response with a list of ticker types.</returns>
    Task<TickerTypesResponse> GetTickerTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a unified list of trade and quote conditions from various market data providers.
    /// These conditions provide context for market data events and affect calculations of trading metrics like high, low, open, close, and volume.
    /// Returns condition codes with their SIP mappings, update rules, and metadata.
    /// </summary>
    /// <param name="assetClass">Filter conditions by asset class. Common values include "stocks", "options", "crypto", "fx".</param>
    /// <param name="dataType">Filter conditions by data type. Common values include "trade", "quote".</param>
    /// <param name="id">Filter by condition ID. Can be a single ID or comma-separated list of IDs.</param>
    /// <param name="sipMapping">Filter by SIP mapping. Specify the mapping type (e.g., "CTA", "UTP", "FINRA_TDDS").</param>
    /// <param name="order">Sort order for the results. Valid values are "asc" (ascending) or "desc" (descending). Default is "asc".</param>
    /// <param name="limit">Limit the number of results returned. Maximum value is 1000. Default is 10.</param>
    /// <param name="sort">Field to sort by. Common values include "asset_class", "data_type", "id", "type", "name". Default is "asset_class".</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a response with a list of condition codes.</returns>
    Task<PolygonResponse<List<ConditionCode>>> GetConditionCodesAsync(
        string? assetClass = null,
        string? dataType = null,
        string? id = null,
        string? sipMapping = null,
        string? order = null,
        int? limit = null,
        string? sort = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a comprehensive list of exchanges and market centers from Polygon.io.
    /// Returns information about trading venues including exchanges, trade reporting facilities (TRFs),
    /// securities information processors (SIPs), and other market-related entities.
    /// </summary>
    /// <param name="assetClass">Filter exchanges by asset class. Common values include "stocks", "options", "crypto", "fx".</param>
    /// <param name="locale">Filter exchanges by locale. Common values include "us" for United States exchanges, "global" for international exchanges.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a response with a list of exchanges.</returns>
    Task<PolygonResponse<List<Exchange>>> GetExchangesAsync(
        string? assetClass = null,
        string? locale = null,
        CancellationToken cancellationToken = default);
}
