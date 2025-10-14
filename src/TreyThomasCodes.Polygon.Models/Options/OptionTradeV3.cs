// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;
using NodaTime;

namespace TreyThomasCodes.Polygon.Models.Options;

/// <summary>
/// Represents a single options trade transaction from the v3/trades endpoint.
/// Contains detailed information about an individual options trade including price, size, exchange, and timing data.
/// This model is specifically designed for the v3/trades/{optionsTicker} endpoint which uses full property names.
/// </summary>
public class OptionTradeV3
{
    /// <summary>
    /// Gets or sets the list of condition codes that apply to this trade.
    /// These codes provide additional context about the nature of the trade (e.g., regular market, extended hours, etc.).
    /// </summary>
    [JsonPropertyName("conditions")]
    public List<int>? Conditions { get; set; }

    /// <summary>
    /// Gets or sets the exchange where this trade was executed.
    /// Represented as a numeric code corresponding to specific exchanges.
    /// </summary>
    [JsonPropertyName("exchange")]
    public int? Exchange { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for this trade.
    /// A trade ID that can be used to reference this specific transaction.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the participant timestamp.
    /// The timestamp from the exchange or market participant that reported this trade.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("participant_timestamp")]
    public long? ParticipantTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the price at which this trade was executed.
    /// The per-contract price in the option's trading currency.
    /// </summary>
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Gets or sets the sequence number for this trade.
    /// Used to order trades that occur at the same timestamp.
    /// </summary>
    [JsonPropertyName("sequence_number")]
    public long? SequenceNumber { get; set; }

    /// <summary>
    /// Gets or sets the SIP timestamp when this trade was reported.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("sip_timestamp")]
    public long? SipTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the number of contracts traded in this transaction.
    /// The volume or quantity of option contracts that changed hands.
    /// </summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }

    /// <summary>
    /// Gets the SIP timestamp converted to Eastern Time.
    /// Returns null if SipTimestamp is null.
    /// </summary>
    [JsonIgnore]
    public ZonedDateTime? MarketTimestamp
    {
        get
        {
            if (!SipTimestamp.HasValue) return null;

            var instant = Instant.FromUnixTimeTicks(SipTimestamp.Value / 100);
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
