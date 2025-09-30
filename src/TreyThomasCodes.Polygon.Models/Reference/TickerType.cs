// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TreyThomasCodes.Polygon.Models.Reference;

/// <summary>
/// Represents a ticker type returned from the Polygon.io ticker types endpoint.
/// Contains information about different security types available in the market.
/// </summary>
public class TickerType
{
    /// <summary>
    /// Gets or sets the code representing the ticker type.
    /// Examples include "CS" for Common Stock, "ETF" for Exchange Traded Fund, "PFD" for Preferred Stock.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable description of the ticker type.
    /// Provides a clear explanation of what the ticker type represents.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the asset class this ticker type belongs to.
    /// Common values include "stocks", "crypto", "forex", "indices".
    /// </summary>
    [JsonPropertyName("asset_class")]
    public string AssetClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locale or geographical region this ticker type applies to.
    /// Common values include "us" for United States, "global" for international markets.
    /// </summary>
    [JsonPropertyName("locale")]
    public string Locale { get; set; } = string.Empty;
}