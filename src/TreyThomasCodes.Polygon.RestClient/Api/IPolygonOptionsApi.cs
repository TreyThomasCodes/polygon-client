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
}
