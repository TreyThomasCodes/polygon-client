// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TreyThomasCodes.Polygon.Models.Stocks;

/// <summary>
/// Represents the result of a last quote (NBBO) request for a stock ticker.
/// Contains the most recent National Best Bid and Offer data.
/// </summary>
public class LastQuoteResult
{
    /// <summary>
    /// Gets or sets the ticker symbol for this quote.
    /// The stock symbol that this NBBO quote represents.
    /// </summary>
    [JsonPropertyName("T")]
    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the bid price.
    /// The highest price a buyer is willing to pay for the security (National Best Bid).
    /// </summary>
    [JsonPropertyName("P")]
    public decimal? BidPrice { get; set; }

    /// <summary>
    /// Gets or sets the ask price.
    /// The lowest price a seller is willing to accept for the security (National Best Offer).
    /// </summary>
    [JsonPropertyName("p")]
    public decimal? AskPrice { get; set; }

    /// <summary>
    /// Gets or sets the bid size.
    /// The number of shares being bid for at the bid price.
    /// </summary>
    [JsonPropertyName("S")]
    public long? BidSize { get; set; }

    /// <summary>
    /// Gets or sets the ask size.
    /// The number of shares being offered at the ask price.
    /// </summary>
    [JsonPropertyName("s")]
    public long? AskSize { get; set; }

    /// <summary>
    /// Gets or sets the bid exchange.
    /// Represented as a numeric code corresponding to specific exchanges.
    /// </summary>
    [JsonPropertyName("X")]
    public int? BidExchange { get; set; }

    /// <summary>
    /// Gets or sets the ask exchange.
    /// Represented as a numeric code corresponding to specific exchanges.
    /// </summary>
    [JsonPropertyName("x")]
    public int? AskExchange { get; set; }

    /// <summary>
    /// Gets or sets the tape designation for this quote.
    /// Indicates which market tape this quote originated from.
    /// </summary>
    [JsonPropertyName("z")]
    public int? Tape { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this quote was generated.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("t")]
    public long? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the participant timestamp.
    /// The timestamp from the exchange or market participant that reported this quote.
    /// </summary>
    [JsonPropertyName("y")]
    public long? ParticipantTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the sequence number for this quote.
    /// Used to order quotes that occur at the same timestamp.
    /// </summary>
    [JsonPropertyName("q")]
    public long? Sequence { get; set; }

    /// <summary>
    /// Gets or sets the list of indicators that apply to this quote.
    /// Provides additional market data context and status information.
    /// </summary>
    [JsonPropertyName("i")]
    public List<int>? Indicators { get; set; }
}
