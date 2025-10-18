// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Options;

/// <summary>
/// Request object for retrieving the most recent trade for a specific options contract.
/// </summary>
public class GetLastTradeRequest
{
    /// <summary>
    /// Gets or sets the options ticker symbol in OCC format (e.g., "O:TSLA260320C00700000").
    /// The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.
    /// </summary>
    public string OptionsTicker { get; set; } = string.Empty;
}
