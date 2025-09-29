// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;
using NodaTime;

namespace TTC.Polygon.Models.Stocks;

/// <summary>
/// Represents a comprehensive snapshot of current market data for a stock ticker.
/// Contains the most recent trade, quote, daily aggregate, and other key market metrics.
/// </summary>
public class StockSnapshot
{
    /// <summary>
    /// Gets or sets the ticker symbol for this snapshot.
    /// The stock symbol that this market snapshot represents.
    /// </summary>
    [JsonPropertyName("ticker")]
    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the current market value or price of the security.
    /// Typically represents the last trade price or current market value.
    /// </summary>
    [JsonPropertyName("value")]
    public decimal? Value { get; set; }

    /// <summary>
    /// Gets or sets the daily aggregate data for the current trading session.
    /// Contains OHLC (Open, High, Low, Close) and volume information for the day.
    /// </summary>
    [JsonPropertyName("day")]
    public DayData? Day { get; set; }

    /// <summary>
    /// Gets or sets the most recent quote data.
    /// Contains the latest bid and ask prices and sizes.
    /// </summary>
    [JsonPropertyName("last_quote")]
    public LastQuoteData? LastQuote { get; set; }

    /// <summary>
    /// Gets or sets the most recent trade data.
    /// Contains information about the last executed trade.
    /// </summary>
    [JsonPropertyName("last_trade")]
    public LastTradeData? LastTrade { get; set; }

    /// <summary>
    /// Gets or sets the most recent minute aggregate data.
    /// Contains OHLC and volume information for the latest minute bar.
    /// </summary>
    [JsonPropertyName("min")]
    public MinuteData? Min { get; set; }

    /// <summary>
    /// Gets or sets the previous trading day's aggregate data.
    /// Contains OHLC and volume information for the prior trading session.
    /// </summary>
    [JsonPropertyName("prevDay")]
    public PreviousDayData? PrevDay { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this snapshot was last updated.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("updated")]
    public long? Updated { get; set; }

    /// <summary>
    /// Gets or sets the percentage change from the previous day's close.
    /// Represents the price change as a percentage.
    /// </summary>
    [JsonPropertyName("todaysChangePerc")]
    public decimal? TodaysChangePerc { get; set; }

    /// <summary>
    /// Gets or sets the absolute change from the previous day's close.
    /// Represents the price change in currency units.
    /// </summary>
    [JsonPropertyName("todaysChange")]
    public decimal? TodaysChange { get; set; }

    /// <summary>
    /// Gets the updated timestamp converted to Eastern Time.
    /// Returns null if Updated is null.
    /// </summary>
    [JsonIgnore]
    public ZonedDateTime? MarketUpdated
    {
        get
        {
            if (!Updated.HasValue) return null;

            var instant = Instant.FromUnixTimeTicks(Updated.Value / 100);
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            return instant.InZone(easternZone);
        }
    }
}

/// <summary>
/// Represents daily aggregate trading data for a stock.
/// Contains OHLC (Open, High, Low, Close), volume, and VWAP information for a trading day.
/// </summary>
public class DayData
{
    /// <summary>
    /// Gets or sets the closing price for the trading day.
    /// The last trade price at the end of the regular trading session.
    /// </summary>
    [JsonPropertyName("c")]
    public decimal? Close { get; set; }

    /// <summary>
    /// Gets or sets the highest price reached during the trading day.
    /// The maximum trade price within the trading session.
    /// </summary>
    [JsonPropertyName("h")]
    public decimal? High { get; set; }

    /// <summary>
    /// Gets or sets the lowest price reached during the trading day.
    /// The minimum trade price within the trading session.
    /// </summary>
    [JsonPropertyName("l")]
    public decimal? Low { get; set; }

    /// <summary>
    /// Gets or sets the opening price for the trading day.
    /// The first trade price at the beginning of the regular trading session.
    /// </summary>
    [JsonPropertyName("o")]
    public decimal? Open { get; set; }

    /// <summary>
    /// Gets or sets the total trading volume for the day.
    /// The total number of shares traded during the trading session.
    /// </summary>
    [JsonPropertyName("v")]
    public ulong? Volume { get; set; }

    /// <summary>
    /// Gets or sets the volume-weighted average price (VWAP) for the trading day.
    /// The average price weighted by the volume of each trade during the session.
    /// </summary>
    [JsonPropertyName("vw")]
    public decimal? VolumeWeightedAveragePrice { get; set; }
}

/// <summary>
/// Represents the most recent quote data for a stock.
/// Contains the latest bid and ask prices, sizes, and timing information.
/// </summary>
public class LastQuoteData
{
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
    /// Gets or sets the timestamp when this quote was generated.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("t")]
    public long? Timestamp { get; set; }

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

/// <summary>
/// Represents the most recent trade data for a stock.
/// Contains information about the last executed trade including price, size, and conditions.
/// </summary>
public class LastTradeData
{
    /// <summary>
    /// Gets or sets the list of condition codes that apply to this trade.
    /// These codes provide additional context about the nature of the trade.
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

/// <summary>
/// Represents minute-level aggregate trading data for a stock.
/// Contains OHLC, volume, and average volume information for a one-minute time period.
/// </summary>
public class MinuteData
{
    /// <summary>
    /// Gets or sets the average volume over a specific period.
    /// Used for comparison against current minute volume levels.
    /// </summary>
    [JsonPropertyName("av")]
    public ulong? AverageVolume { get; set; }

    /// <summary>
    /// Gets or sets the closing price for the minute period.
    /// The last trade price at the end of the minute.
    /// </summary>
    [JsonPropertyName("c")]
    public decimal? Close { get; set; }

    /// <summary>
    /// Gets or sets the highest price reached during the minute period.
    /// The maximum trade price within the minute.
    /// </summary>
    [JsonPropertyName("h")]
    public decimal? High { get; set; }

    /// <summary>
    /// Gets or sets the lowest price reached during the minute period.
    /// The minimum trade price within the minute.
    /// </summary>
    [JsonPropertyName("l")]
    public decimal? Low { get; set; }

    /// <summary>
    /// Gets or sets the opening price for the minute period.
    /// The first trade price at the beginning of the minute.
    /// </summary>
    [JsonPropertyName("o")]
    public decimal? Open { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for the start of this minute period.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("t")]
    public long? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the trading volume for the minute period.
    /// The total number of shares traded during the minute.
    /// </summary>
    [JsonPropertyName("v")]
    public ulong? Volume { get; set; }

    /// <summary>
    /// Gets or sets the volume-weighted average price (VWAP) for the minute period.
    /// The average price weighted by the volume of each trade during the minute.
    /// </summary>
    [JsonPropertyName("vw")]
    public decimal? VolumeWeightedAveragePrice { get; set; }

    /// <summary>
    /// Gets or sets the number of transactions in the minute period.
    /// The total count of individual trades that occurred during the minute.
    /// </summary>
    [JsonPropertyName("n")]
    public int? TransactionCount { get; set; }

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

/// <summary>
/// Represents aggregate trading data for the previous trading day.
/// Contains OHLC, volume, and VWAP information for the prior trading session.
/// </summary>
public class PreviousDayData
{
    /// <summary>
    /// Gets or sets the closing price for the previous trading day.
    /// The last trade price at the end of the prior trading session.
    /// </summary>
    [JsonPropertyName("c")]
    public decimal? Close { get; set; }

    /// <summary>
    /// Gets or sets the highest price reached during the previous trading day.
    /// The maximum trade price within the prior trading session.
    /// </summary>
    [JsonPropertyName("h")]
    public decimal? High { get; set; }

    /// <summary>
    /// Gets or sets the lowest price reached during the previous trading day.
    /// The minimum trade price within the prior trading session.
    /// </summary>
    [JsonPropertyName("l")]
    public decimal? Low { get; set; }

    /// <summary>
    /// Gets or sets the opening price for the previous trading day.
    /// The first trade price at the beginning of the prior trading session.
    /// </summary>
    [JsonPropertyName("o")]
    public decimal? Open { get; set; }

    /// <summary>
    /// Gets or sets the total trading volume for the previous day.
    /// The total number of shares traded during the prior trading session.
    /// </summary>
    [JsonPropertyName("v")]
    public ulong? Volume { get; set; }

    /// <summary>
    /// Gets or sets the volume-weighted average price (VWAP) for the previous trading day.
    /// The average price weighted by the volume of each trade during the prior session.
    /// </summary>
    [JsonPropertyName("vw")]
    public decimal? VolumeWeightedAveragePrice { get; set; }
}

/// <summary>
/// Represents the response from the individual ticker snapshot API endpoint.
/// Contains the snapshot data wrapped in a specific response format.
/// </summary>
public class StockSnapshotResponse
{
    /// <summary>
    /// Gets or sets the ticker snapshot data.
    /// Contains all market data for the specific ticker symbol.
    /// </summary>
    [JsonPropertyName("ticker")]
    public StockSnapshot? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the status of the API response.
    /// Indicates whether the request was successful.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for this API request.
    /// Can be used for debugging and request tracking purposes.
    /// </summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }
}