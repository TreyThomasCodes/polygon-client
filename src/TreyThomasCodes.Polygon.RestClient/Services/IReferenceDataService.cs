// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0


using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.Models.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

namespace TreyThomasCodes.Polygon.RestClient.Services;

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
    /// <param name="request">The request containing filtering and sorting parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of ticker records.</returns>
    Task<PolygonResponse<List<StockTicker>>> GetTickersAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information for a specific ticker symbol.
    /// Returns comprehensive ticker metadata including company information, exchange details, and identifiers.
    /// </summary>
    /// <param name="request">The request containing the ticker and optional date parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing detailed ticker information.</returns>
    Task<PolygonResponse<StockTicker>> GetTickerDetailsAsync(
        GetTickerDetailsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information for a specific ticker symbol.
    /// Returns comprehensive ticker metadata including company information, exchange details, and identifiers.
    /// This is a convenience overload that accepts the ticker symbol directly.
    /// </summary>
    /// <param name="ticker">The ticker symbol to retrieve details for (e.g., "AAPL", "MSFT").</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing detailed ticker information.</returns>
    Task<PolygonResponse<StockTicker>> GetTickerDetailsAsync(
        string ticker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current trading status for various exchanges and overall financial markets.
    /// Returns real-time information about market hours, pre-market and after-hours sessions,
    /// and the status of individual exchanges and index groups.
    /// </summary>
    /// <param name="request">The request (no parameters required).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the current market status information.</returns>
    Task<MarketStatus> GetMarketStatusAsync(
        GetMarketStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all ticker types supported by Polygon.io.
    /// Returns information about different security types including their codes, descriptions, asset classes, and locales.
    /// Examples include "CS" for Common Stock, "ETF" for Exchange Traded Fund, "PFD" for Preferred Stock.
    /// </summary>
    /// <param name="request">The request (no parameters required).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a response with a list of ticker types.</returns>
    Task<TickerTypesResponse> GetTickerTypesAsync(
        GetTickerTypesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a unified list of trade and quote conditions from various market data providers.
    /// These conditions provide context for market data events and affect calculations of trading metrics like high, low, open, close, and volume.
    /// Returns condition codes with their SIP mappings, update rules, and metadata.
    /// </summary>
    /// <param name="request">The request containing filtering and sorting parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a response with a list of condition codes.</returns>
    Task<PolygonResponse<List<ConditionCode>>> GetConditionCodesAsync(
        GetConditionCodesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a comprehensive list of exchanges and market centers from Polygon.io.
    /// Returns information about trading venues including exchanges, trade reporting facilities (TRFs),
    /// securities information processors (SIPs), and other market-related entities.
    /// </summary>
    /// <param name="request">The request containing filtering parameters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a response with a list of exchanges.</returns>
    Task<PolygonResponse<List<Exchange>>> GetExchangesAsync(
        GetExchangesRequest request,
        CancellationToken cancellationToken = default);
}
