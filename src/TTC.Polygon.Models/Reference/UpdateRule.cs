// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TTC.Polygon.Models.Reference;

/// <summary>
/// Represents the update rules for a condition code, indicating what market metrics are affected.
/// Determines whether trades with this condition update high/low, open/close, and volume calculations.
/// </summary>
public class UpdateRule
{
    /// <summary>
    /// Gets or sets a value indicating whether trades with this condition update the high and low price calculations.
    /// True if the condition affects daily high/low tracking, false otherwise.
    /// </summary>
    [JsonPropertyName("updates_high_low")]
    public bool UpdatesHighLow { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether trades with this condition update the open and close price calculations.
    /// True if the condition affects opening and closing price determination, false otherwise.
    /// </summary>
    [JsonPropertyName("updates_open_close")]
    public bool UpdatesOpenClose { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether trades with this condition update the volume calculations.
    /// True if the condition contributes to total volume, false otherwise.
    /// </summary>
    [JsonPropertyName("updates_volume")]
    public bool UpdatesVolume { get; set; }
}