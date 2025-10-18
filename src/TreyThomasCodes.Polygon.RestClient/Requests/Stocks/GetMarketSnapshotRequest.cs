// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

/// <summary>
/// Request object for retrieving current market snapshot data for all US stock tickers.
/// </summary>
public class GetMarketSnapshotRequest
{
    /// <summary>
    /// Gets or sets whether to include over-the-counter (OTC) securities in the results. Defaults to false if not specified.
    /// </summary>
    public bool? IncludeOtc { get; set; }
}
