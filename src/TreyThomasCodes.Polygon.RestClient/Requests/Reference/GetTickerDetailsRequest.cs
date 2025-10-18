// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Reference;

/// <summary>
/// Request object for retrieving detailed information for a specific ticker symbol.
/// </summary>
public class GetTickerDetailsRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol for which to retrieve detailed information (e.g., "AAPL", "MSFT").
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date to retrieve ticker details as of in YYYY-MM-DD format.
    /// Defaults to the most recent available data if not specified.
    /// </summary>
    public string? Date { get; set; }
}
