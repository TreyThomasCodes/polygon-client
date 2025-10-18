// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;

namespace TreyThomasCodes.Polygon.RestClient.Requests.Reference;

/// <summary>
/// Request object for retrieving a comprehensive list of exchanges and market centers from Polygon.io.
/// </summary>
public class GetExchangesRequest
{
    /// <summary>
    /// Gets or sets the filter for exchanges by asset class.
    /// </summary>
    public AssetClass? AssetClass { get; set; }

    /// <summary>
    /// Gets or sets the filter for exchanges by locale.
    /// </summary>
    public Locale? Locale { get; set; }
}
