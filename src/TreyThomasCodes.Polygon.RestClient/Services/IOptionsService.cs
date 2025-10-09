// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;

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
    /// <param name="optionsTicker">The options ticker symbol in OCC format (e.g., "O:SPY251219C00650000"). The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract details including CFI code, contract type, exercise style, expiration date, primary exchange, shares per contract, strike price, ticker, and underlying ticker.</returns>
    Task<PolygonResponse<OptionsContract>> GetContractDetailsAsync(
        string optionsTicker,
        CancellationToken cancellationToken = default);
}
