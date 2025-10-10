// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;
using NodaTime;

namespace TreyThomasCodes.Polygon.Models.Options;

/// <summary>
/// Represents a single options quote containing bid and ask information.
/// Contains real-time bid/ask prices, sizes, exchange information, and timing data for an options contract.
/// </summary>
public class OptionQuote
{
    /// <summary>
    /// Gets or sets the ask exchange.
    /// Represented as a numeric code corresponding to the specific exchange offering the ask.
    /// </summary>
    [JsonPropertyName("ask_exchange")]
    public int? AskExchange { get; set; }

    /// <summary>
    /// Gets or sets the ask price.
    /// The lowest price a seller is willing to accept for the option contract.
    /// </summary>
    [JsonPropertyName("ask_price")]
    public decimal? AskPrice { get; set; }

    /// <summary>
    /// Gets or sets the ask size.
    /// The number of contracts being offered at the ask price.
    /// </summary>
    [JsonPropertyName("ask_size")]
    public int? AskSize { get; set; }

    /// <summary>
    /// Gets or sets the bid exchange.
    /// Represented as a numeric code corresponding to the specific exchange offering the bid.
    /// </summary>
    [JsonPropertyName("bid_exchange")]
    public int? BidExchange { get; set; }

    /// <summary>
    /// Gets or sets the bid price.
    /// The highest price a buyer is willing to pay for the option contract.
    /// </summary>
    [JsonPropertyName("bid_price")]
    public decimal? BidPrice { get; set; }

    /// <summary>
    /// Gets or sets the bid size.
    /// The number of contracts being bid for at the bid price.
    /// </summary>
    [JsonPropertyName("bid_size")]
    public int? BidSize { get; set; }

    /// <summary>
    /// Gets or sets the sequence number for this quote.
    /// Used to order quotes that occur at the same timestamp.
    /// </summary>
    [JsonPropertyName("sequence_number")]
    public long? SequenceNumber { get; set; }

    /// <summary>
    /// Gets or sets the SIP (Securities Information Processor) timestamp when this quote was generated.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("sip_timestamp")]
    public long? SipTimestamp { get; set; }

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
}
