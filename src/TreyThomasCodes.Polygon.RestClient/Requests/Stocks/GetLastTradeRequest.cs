// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

/// <summary>
/// Request object for retrieving the most recent trade for a specific stock ticker.
/// </summary>
public class GetLastTradeRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol for which to retrieve the last trade (e.g., "AAPL", "MSFT").
    /// </summary>
    public string Ticker { get; set; } = string.Empty;
}
