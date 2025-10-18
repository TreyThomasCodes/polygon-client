// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

/// <summary>
/// Request object for retrieving the previous trading day's OHLC data for a specific stock ticker.
/// </summary>
public class GetPreviousCloseRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol for which to retrieve previous close data (e.g., "AAPL", "MSFT").
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to adjust for stock splits and dividend payments. Defaults to true if not specified.
    /// </summary>
    public bool? Adjusted { get; set; }
}
