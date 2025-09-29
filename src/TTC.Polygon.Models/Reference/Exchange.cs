// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TTC.Polygon.Models.Reference;

/// <summary>
/// Represents an exchange returned from the Polygon.io exchanges endpoint.
/// Contains information about trading venues, market centers, and other exchange-related entities.
/// </summary>
public class Exchange
{
    /// <summary>
    /// Gets or sets the unique identifier for this exchange.
    /// This is a numeric ID that uniquely identifies the exchange within Polygon.io's system.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the type of exchange entity.
    /// Common values include "exchange", "TRF" (Trade Reporting Facility), "SIP" (Securities Information Processor), and "ORF" (OTC Reporting Facility).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the asset class this exchange handles.
    /// Common values include "stocks", "options", "forex", "crypto".
    /// </summary>
    [JsonPropertyName("asset_class")]
    public string AssetClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locale or geographic region where this exchange operates.
    /// Common values include "us" for United States exchanges and "global" for international or multi-regional exchanges.
    /// </summary>
    [JsonPropertyName("locale")]
    public string Locale { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full official name of the exchange.
    /// This is the complete legal or trading name of the exchange (e.g., "New York Stock Exchange", "Nasdaq").
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the acronym or abbreviated name of the exchange.
    /// This is a shortened version of the exchange name (e.g., "NYSE", "AMEX", "NSX").
    /// May be null if no acronym is defined for this exchange.
    /// </summary>
    [JsonPropertyName("acronym")]
    public string? Acronym { get; set; }

    /// <summary>
    /// Gets or sets the Market Identifier Code (MIC) for this exchange.
    /// This is a four-character ISO 10383 standard code that uniquely identifies the exchange (e.g., "XNYS", "XNAS").
    /// May be null for certain types of exchanges or reporting facilities.
    /// </summary>
    [JsonPropertyName("mic")]
    public string? Mic { get; set; }

    /// <summary>
    /// Gets or sets the operating Market Identifier Code (MIC) for this exchange.
    /// This identifies the entity that operates the exchange, which may be different from the exchange's own MIC.
    /// For example, multiple exchanges may be operated by the same parent company.
    /// </summary>
    [JsonPropertyName("operating_mic")]
    public string OperatingMic { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the participant identifier for this exchange.
    /// This is typically a single character used to identify the exchange in market data feeds and trading systems.
    /// May be null for certain types of exchanges that don't have participant IDs.
    /// </summary>
    [JsonPropertyName("participant_id")]
    public string? ParticipantId { get; set; }

    /// <summary>
    /// Gets or sets the official website URL for this exchange.
    /// Provides a link to the exchange's main website for additional information.
    /// May be null if no website URL is available.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}