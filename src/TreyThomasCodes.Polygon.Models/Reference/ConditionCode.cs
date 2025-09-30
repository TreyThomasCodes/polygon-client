// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TreyThomasCodes.Polygon.Models.Reference;

/// <summary>
/// Represents a trade or quote condition code returned from the Polygon.io conditions endpoint.
/// Contains information about market data conditions that provide context for trades and quotes.
/// </summary>
public class ConditionCode
{
    /// <summary>
    /// Gets or sets the unique identifier for this condition code.
    /// This is a numeric ID that uniquely identifies the condition across all asset classes.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the type of condition code.
    /// Common values include "sale_condition" for trade conditions and "quote_condition" for quote conditions.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable name of the condition code.
    /// Provides a descriptive name for what this condition represents (e.g., "Cash Sale", "Cross Trade").
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the asset class this condition code applies to.
    /// Common values include "stocks", "options", "forex", "crypto".
    /// </summary>
    [JsonPropertyName("asset_class")]
    public string AssetClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SIP (Securities Information Processor) mapping for this condition.
    /// Maps condition codes to their corresponding symbols across different market data feeds.
    /// </summary>
    [JsonPropertyName("sip_mapping")]
    public SipMapping SipMapping { get; set; } = new();

    /// <summary>
    /// Gets or sets the update rules for this condition code.
    /// Determines how trades or quotes with this condition affect market calculations like high/low, open/close, and volume.
    /// </summary>
    [JsonPropertyName("update_rules")]
    public UpdateRules UpdateRules { get; set; } = new();

    /// <summary>
    /// Gets or sets the data types this condition code can apply to.
    /// Common values include "trade" for trade conditions and "quote" for quote conditions.
    /// </summary>
    [JsonPropertyName("data_types")]
    public List<string> DataTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether this condition code is considered legacy.
    /// Legacy conditions are older condition codes that may be deprecated or less commonly used.
    /// </summary>
    [JsonPropertyName("legacy")]
    public bool? Legacy { get; set; }
}