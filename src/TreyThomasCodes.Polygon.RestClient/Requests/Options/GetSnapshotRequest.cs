// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Options;

/// <summary>
/// Request object for retrieving a snapshot of current market data for a specific options contract.
/// </summary>
public class GetSnapshotRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol of the underlying asset (e.g., "SPY", "AAPL").
    /// </summary>
    public string UnderlyingAsset { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the options contract identifier in OCC format without the "O:" prefix (e.g., "SPY251219C00650000").
    /// </summary>
    public string OptionContract { get; set; } = string.Empty;
}
