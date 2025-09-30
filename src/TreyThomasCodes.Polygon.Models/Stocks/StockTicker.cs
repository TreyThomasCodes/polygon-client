// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TreyThomasCodes.Polygon.Models.Stocks;

/// <summary>
/// Represents a stock ticker with comprehensive metadata and identifiers.
/// Contains information about a financial security including its symbol, name, exchange, and various identifiers.
/// </summary>
public class StockTicker
{
    /// <summary>
    /// Gets or sets the ticker symbol (e.g., "AAPL", "MSFT").
    /// This is the primary identifier used to reference the security in trading and data requests.
    /// </summary>
    [JsonPropertyName("ticker")]
    public string? Symbol { get; set; }

    /// <summary>
    /// Gets or sets the full company or security name.
    /// Provides the human-readable name of the entity behind the ticker symbol.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the market category (e.g., "stocks", "indices", "forex").
    /// Indicates the broad market classification for this security.
    /// </summary>
    [JsonPropertyName("market")]
    public string? Market { get; set; }

    /// <summary>
    /// Gets or sets the locale or geographic region (e.g., "us", "global").
    /// Indicates the geographic market where this security is primarily traded.
    /// </summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the primary exchange where the security is listed (e.g., "NASDAQ", "NYSE").
    /// This is the main trading venue for the security.
    /// </summary>
    [JsonPropertyName("primary_exchange")]
    public string? PrimaryExchange { get; set; }

    /// <summary>
    /// Gets or sets the security type (e.g., "CS" for Common Stock, "ETF" for Exchange-Traded Fund).
    /// Classifies the financial instrument type according to standard market conventions.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets whether the ticker is currently active and tradable.
    /// False indicates the security is delisted or no longer trading.
    /// </summary>
    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    /// <summary>
    /// Gets or sets the currency in which the security is priced (e.g., "USD", "EUR").
    /// Indicates the base currency for all price and value data.
    /// </summary>
    [JsonPropertyName("currency_name")]
    public string? CurrencyName { get; set; }

    /// <summary>
    /// Gets or sets the Central Index Key (CIK) identifier.
    /// A unique identifier assigned by the SEC to entities that file disclosures.
    /// </summary>
    [JsonPropertyName("cik")]
    public string? Cik { get; set; }

    /// <summary>
    /// Gets or sets the composite Financial Instrument Global Identifier (FIGI).
    /// A globally unique identifier for financial securities across all markets and exchanges.
    /// </summary>
    [JsonPropertyName("composite_figi")]
    public string? CompositeFigi { get; set; }

    /// <summary>
    /// Gets or sets the share class specific FIGI identifier.
    /// Identifies the specific share class when a company has multiple share classes.
    /// </summary>
    [JsonPropertyName("share_class_figi")]
    public string? ShareClassFigi { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this ticker information was last updated (UTC).
    /// Indicates the freshness of the metadata for this security.
    /// </summary>
    [JsonPropertyName("last_updated_utc")]
    public string? LastUpdatedUtc { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this ticker was delisted (UTC).
    /// Only present for securities that are no longer actively traded.
    /// </summary>
    [JsonPropertyName("delisted_utc")]
    public string? DelistedUtc { get; set; }
}