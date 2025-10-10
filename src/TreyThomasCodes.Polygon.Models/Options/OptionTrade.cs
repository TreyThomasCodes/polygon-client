// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;
using NodaTime;

namespace TreyThomasCodes.Polygon.Models.Options;

/// <summary>
/// Represents a single options trade transaction.
/// Contains detailed information about an individual options trade including price, size, exchange, and timing data.
/// </summary>
public class OptionTrade
{
    /// <summary>
    /// Gets or sets the ticker symbol for this options trade.
    /// The options contract ticker that this trade was executed for, in OCC format (e.g., "O:TSLA260320C00700000").
    /// </summary>
    [JsonPropertyName("T")]
    public string? Ticker { get; set; }

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
    /// Gets or sets the price at which this trade was executed.
    /// The per-contract price in the option's trading currency.
    /// </summary>
    [JsonPropertyName("p")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Gets or sets the sequence number for this trade.
    /// Used to order trades that occur at the same timestamp.
    /// </summary>
    [JsonPropertyName("q")]
    public long? Sequence { get; set; }

    /// <summary>
    /// Gets or sets the number of contracts traded in this transaction.
    /// The volume or quantity of option contracts that changed hands.
    /// </summary>
    [JsonPropertyName("s")]
    public int? Size { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this trade was executed.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("t")]
    public long? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the exchange where this trade was executed.
    /// Represented as a numeric code corresponding to specific exchanges.
    /// </summary>
    [JsonPropertyName("x")]
    public int? Exchange { get; set; }

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
}
