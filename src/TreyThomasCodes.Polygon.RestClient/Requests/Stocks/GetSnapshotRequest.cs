// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

/// <summary>
/// Request object for retrieving current market snapshot data for a specific stock ticker.
/// </summary>
public class GetSnapshotRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol for which to retrieve snapshot data (e.g., "AAPL", "MSFT").
    /// </summary>
    public string Ticker { get; set; } = string.Empty;
}
