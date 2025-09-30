// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;
using NodaTime;

namespace TreyThomasCodes.Polygon.Models.Stocks;

/// <summary>
/// Represents an aggregate (OHLC) bar for a stock ticker.
/// Contains Open, High, Low, Close, Volume, and other trading data for a specific time period.
/// </summary>
public class StockBar
{
    /// <summary>
    /// Gets or sets the ticker symbol for this aggregate data.
    /// The stock symbol that this OHLC data represents.
    /// </summary>
    [JsonPropertyName("T")]
    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the trading volume for this time period.
    /// The total number of shares traded during the aggregate window.
    /// </summary>
    [JsonPropertyName("v")]
    public ulong? Volume { get; set; }

    /// <summary>
    /// Gets or sets the volume-weighted average price (VWAP) for this time period.
    /// The average price weighted by the volume of each trade during the aggregate window.
    /// </summary>
    [JsonPropertyName("vw")]
    public decimal? VolumeWeightedAveragePrice { get; set; }

    /// <summary>
    /// Gets or sets the opening price for this time period.
    /// The first trade price at the beginning of the aggregate window.
    /// </summary>
    [JsonPropertyName("o")]
    public decimal? Open { get; set; }

    /// <summary>
    /// Gets or sets the closing price for this time period.
    /// The last trade price at the end of the aggregate window.
    /// </summary>
    [JsonPropertyName("c")]
    public decimal? Close { get; set; }

    /// <summary>
    /// Gets or sets the highest price reached during this time period.
    /// The maximum trade price within the aggregate window.
    /// </summary>
    [JsonPropertyName("h")]
    public decimal? High { get; set; }

    /// <summary>
    /// Gets or sets the lowest price reached during this time period.
    /// The minimum trade price within the aggregate window.
    /// </summary>
    [JsonPropertyName("l")]
    public decimal? Low { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for the start of this aggregate window.
    /// Expressed as milliseconds since the Unix epoch in the market's location (January 1, 1970 EST for NYSE).
    /// </summary>
    [JsonPropertyName("t")]
    public ulong? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the number of individual transactions that occurred during this time period.
    /// The count of separate trades that were aggregated to create this bar.
    /// </summary>
    [JsonPropertyName("n")]
    public int? NumberOfTransactions { get; set; }

    /// <summary>
    /// Gets the timestamp converted to Eastern Time.
    /// Returns null if Timestamp is null.
    /// </summary>
    [JsonIgnore]
    public ZonedDateTime? MarketTimestamp
    {
        get
        {
            if (!Timestamp.HasValue) return null;

            var instant = Instant.FromUnixTimeMilliseconds((long)Timestamp.Value);
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            return instant.InZone(easternZone);
        }
    }
}