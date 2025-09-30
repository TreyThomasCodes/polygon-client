// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TreyThomasCodes.Polygon.Models.Reference;

/// <summary>
/// Represents the SIP (Securities Information Processor) mapping for a condition code.
/// Maps condition codes to their corresponding symbols across different market data feeds.
/// </summary>
public class SipMapping
{
    /// <summary>
    /// Gets or sets the CTA (Consolidated Tape Association) symbol for this condition.
    /// Used for NYSE and other CTA markets.
    /// </summary>
    [JsonPropertyName("CTA")]
    public string? Cta { get; set; }

    /// <summary>
    /// Gets or sets the UTP (Unlisted Trading Privileges) symbol for this condition.
    /// Used for NASDAQ and other UTP markets.
    /// </summary>
    [JsonPropertyName("UTP")]
    public string? Utp { get; set; }

    /// <summary>
    /// Gets or sets the FINRA TDDS (Trade Data Dissemination Service) symbol for this condition.
    /// Used for over-the-counter and FINRA-regulated markets.
    /// </summary>
    [JsonPropertyName("FINRA_TDDS")]
    public string? FinraTdds { get; set; }
}