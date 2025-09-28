// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;
using NodaTime;

namespace TTC.Polygon.Models.Stocks;

/// <summary>
/// Represents a stock quote with bid and ask information.
/// Contains real-time or historical bid/ask prices, sizes, and related market data.
/// </summary>
public class StockQuote
{
    /// <summary>
    /// Gets or sets the ticker symbol for this quote.
    /// The stock symbol that this bid/ask quote represents.
    /// </summary>
    [JsonPropertyName("T")]
    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this quote was generated.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("t")]
    public long? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the start of the timeframe for this quote.
    /// Used for aggregate data and represents the beginning of the time window.
    /// </summary>
    [JsonPropertyName("f")]
    public long? TimeframeStart { get; set; }

    /// <summary>
    /// Gets or sets the sequence number for this quote.
    /// Used to order quotes that occur at the same timestamp.
    /// </summary>
    [JsonPropertyName("q")]
    public long? Sequence { get; set; }

    /// <summary>
    /// Gets or sets the participant timestamp.
    /// The timestamp from the exchange or market participant that reported this quote.
    /// </summary>
    [JsonPropertyName("y")]
    public long? ParticipantTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the list of condition codes that apply to this quote.
    /// These codes provide additional context about the nature of the quote.
    /// </summary>
    [JsonPropertyName("c")]
    public List<int>? Conditions { get; set; }

    /// <summary>
    /// Gets or sets the list of indicators that apply to this quote.
    /// Provides additional market data context and status information.
    /// </summary>
    [JsonPropertyName("i")]
    public List<int>? Indicators { get; set; }

    /// <summary>
    /// Gets or sets the bid price.
    /// The highest price a buyer is willing to pay for the security.
    /// </summary>
    [JsonPropertyName("P")]
    public decimal? BidPrice { get; set; }

    /// <summary>
    /// Gets or sets the bid size.
    /// The number of shares being bid for at the bid price.
    /// </summary>
    [JsonPropertyName("S")]
    public long? BidSize { get; set; }

    /// <summary>
    /// Gets or sets the ask price.
    /// The lowest price a seller is willing to accept for the security.
    /// </summary>
    [JsonPropertyName("p")]
    public decimal? AskPrice { get; set; }

    /// <summary>
    /// Gets or sets the ask size.
    /// The number of shares being offered at the ask price.
    /// </summary>
    [JsonPropertyName("s")]
    public long? AskSize { get; set; }

    /// <summary>
    /// Gets or sets the exchange where the bid was placed.
    /// Represented as a numeric code corresponding to specific exchanges.
    /// </summary>
    [JsonPropertyName("x")]
    public int? BidExchange { get; set; }

    /// <summary>
    /// Gets or sets the exchange where the ask was placed.
    /// Represented as a numeric code corresponding to specific exchanges.
    /// </summary>
    [JsonPropertyName("X")]
    public int? AskExchange { get; set; }

    /// <summary>
    /// Gets or sets the tape designation for this quote.
    /// Indicates which market tape this quote originated from.
    /// </summary>
    [JsonPropertyName("z")]
    public int? Tape { get; set; }

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