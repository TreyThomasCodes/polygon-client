// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TTC.Polygon.Models.Reference;

/// <summary>
/// Represents the update rules for both consolidated and market center calculations.
/// Contains separate rules for how a condition affects consolidated market data versus individual market center data.
/// </summary>
public class UpdateRules
{
    /// <summary>
    /// Gets or sets the update rules for consolidated market data calculations.
    /// These rules apply to market-wide aggregated data across all exchanges.
    /// </summary>
    [JsonPropertyName("consolidated")]
    public UpdateRule Consolidated { get; set; } = new();

    /// <summary>
    /// Gets or sets the update rules for individual market center calculations.
    /// These rules apply to data specific to each exchange or market center.
    /// </summary>
    [JsonPropertyName("market_center")]
    public UpdateRule MarketCenter { get; set; } = new();
}