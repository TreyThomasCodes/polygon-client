// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Options;

/// <summary>
/// Request object for retrieving the previous trading day's OHLC data for a specific options contract.
/// </summary>
public class GetPreviousDayBarRequest
{
    /// <summary>
    /// Gets or sets the options ticker symbol in OCC format (e.g., "O:SPY251219C00650000").
    /// The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.
    /// </summary>
    public string OptionsTicker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to adjust for underlying stock splits. Defaults to true if not specified.
    /// Note that options contracts themselves are not adjusted for splits, but this parameter affects how the underlying asset's price changes are reflected.
    /// </summary>
    public bool? Adjusted { get; set; }
}
