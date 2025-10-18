// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

/// <summary>
/// Request object for retrieving the daily open and close prices for a specific stock ticker on a given date.
/// </summary>
public class GetDailyOpenCloseRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol for which to retrieve daily open/close data (e.g., "AAPL", "MSFT").
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date for which to retrieve open/close data in YYYY-MM-DD format.
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to adjust for stock splits and dividend payments. Defaults to true if not specified.
    /// </summary>
    public bool? Adjusted { get; set; }
}
