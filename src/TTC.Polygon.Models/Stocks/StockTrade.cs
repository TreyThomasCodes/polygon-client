// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;
using NodaTime;

namespace TTC.Polygon.Models.Stocks;

/// <summary>
/// Represents a single stock trade transaction.
/// Contains detailed information about an individual trade including price, size, exchange, and timing data.
/// </summary>
public class StockTrade
{
    /// <summary>
    /// Gets or sets the ticker symbol for this trade.
    /// The stock symbol that this trade was executed for.
    /// </summary>
    [JsonPropertyName("T")]
    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this trade was executed.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("t")]
    public long? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the start of the timeframe for this trade.
    /// Used for aggregate data and represents the beginning of the time window.
    /// </summary>
    [JsonPropertyName("f")]
    public long? TimeframeStart { get; set; }

    /// <summary>
    /// Gets or sets the sequence number for this trade.
    /// Used to order trades that occur at the same timestamp.
    /// </summary>
    [JsonPropertyName("q")]
    public long? Sequence { get; set; }

    /// <summary>
    /// Gets or sets the participant timestamp.
    /// The timestamp from the exchange or market participant that reported this trade.
    /// </summary>
    [JsonPropertyName("y")]
    public long? ParticipantTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the list of condition codes that apply to this trade.
    /// These codes provide additional context about the nature of the trade (e.g., regular market, extended hours, etc.).
    /// </summary>
    [JsonPropertyName("c")]
    public List<int>? Conditions { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for this trade.
    /// A trade ID that can be used to reference this specific transaction.
    /// </summary>
    [JsonPropertyName("i")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the exchange where this trade was executed.
    /// Represented as a numeric code corresponding to specific exchanges.
    /// </summary>
    [JsonPropertyName("x")]
    public int? Exchange { get; set; }

    /// <summary>
    /// Gets or sets the price at which this trade was executed.
    /// The per-share price in the security's trading currency.
    /// </summary>
    [JsonPropertyName("p")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Gets or sets the number of shares traded in this transaction.
    /// The volume or quantity of the security that changed hands.
    /// </summary>
    [JsonPropertyName("s")]
    public long? Size { get; set; }

    /// <summary>
    /// Gets or sets the tape designation or correction indicator.
    /// Indicates which market tape the trade occurred on or if this is a correction to a previous trade.
    /// </summary>
    [JsonPropertyName("r")]
    public int? TapeOrCorrection { get; set; }

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

            var instant = Instant.FromUnixTimeTicks(Timestamp.Value / 100);
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            return instant.InZone(easternZone);
        }
    }

    /// <summary>
    /// Gets the timeframe start converted to Eastern Time.
    /// Returns null if TimeframeStart is null.
    /// </summary>
    [JsonIgnore]
    public ZonedDateTime? MarketTimeframeStart
    {
        get
        {
            if (!TimeframeStart.HasValue) return null;

            var instant = Instant.FromUnixTimeTicks(TimeframeStart.Value / 100);
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            return instant.InZone(easternZone);
        }
    }

    /// <summary>
    /// Gets the participant timestamp converted to Eastern Time.
    /// Returns null if ParticipantTimestamp is null.
    /// </summary>
    [JsonIgnore]
    public ZonedDateTime? MarketParticipantTimestamp
    {
        get
        {
            if (!ParticipantTimestamp.HasValue) return null;

            var instant = Instant.FromUnixTimeTicks(ParticipantTimestamp.Value / 100);
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            return instant.InZone(easternZone);
        }
    }
}