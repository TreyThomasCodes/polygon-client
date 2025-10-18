// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

/// <summary>
/// Request object for retrieving the most recent National Best Bid and Offer (NBBO) quote for a specific stock ticker.
/// </summary>
public class GetLastQuoteRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol for which to retrieve the last quote (e.g., "AAPL", "MSFT").
    /// </summary>
    public string Ticker { get; set; } = string.Empty;
}
