// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Options;

/// <summary>
/// Request object for retrieving the daily open, high, low, close (OHLC) summary for a specific options contract on a given date.
/// </summary>
public class GetDailyOpenCloseRequest
{
    /// <summary>
    /// Gets or sets the options ticker symbol in OCC format (e.g., "O:SPY251219C00650000").
    /// The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.
    /// </summary>
    public string OptionsTicker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the requested daily data in YYYY-MM-DD format (e.g., "2023-01-09").
    /// </summary>
    public string Date { get; set; } = string.Empty;
}
