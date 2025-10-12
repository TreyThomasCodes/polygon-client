// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TreyThomasCodes.Polygon.Models.Options;

/// <summary>
/// Represents the daily open, high, low, and close (OHLC) data for a specific options contract on a given date.
/// Includes pre-market and after-hours pricing information along with trading volume.
/// </summary>
public class OptionDailyOpenClose
{
    /// <summary>
    /// Gets or sets the status of the API response.
    /// Typically "OK" for successful requests.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the date for this daily summary in YYYY-MM-DD format.
    /// Represents the trading day for which the OHLC data applies.
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>
    /// Gets or sets the options contract ticker symbol in OCC format.
    /// Example: "O:SPY251219C00650000" for a SPY call option expiring December 19, 2025 with a $650 strike price.
    /// </summary>
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    /// <summary>
    /// Gets or sets the opening price for the trading day.
    /// The first trade price when regular market hours began.
    /// </summary>
    [JsonPropertyName("open")]
    public decimal? Open { get; set; }

    /// <summary>
    /// Gets or sets the highest price reached during the trading day.
    /// The maximum trade price within regular market hours.
    /// </summary>
    [JsonPropertyName("high")]
    public decimal? High { get; set; }

    /// <summary>
    /// Gets or sets the lowest price reached during the trading day.
    /// The minimum trade price within regular market hours.
    /// </summary>
    [JsonPropertyName("low")]
    public decimal? Low { get; set; }

    /// <summary>
    /// Gets or sets the closing price for the trading day.
    /// The last trade price when regular market hours ended.
    /// </summary>
    [JsonPropertyName("close")]
    public decimal? Close { get; set; }

    /// <summary>
    /// Gets or sets the total trading volume for the day.
    /// The total number of contracts traded during regular market hours.
    /// </summary>
    [JsonPropertyName("volume")]
    public ulong? Volume { get; set; }

    /// <summary>
    /// Gets or sets the after-hours trading price.
    /// The last trade price from the after-hours trading session.
    /// </summary>
    [JsonPropertyName("afterHours")]
    public decimal? AfterHours { get; set; }

    /// <summary>
    /// Gets or sets the pre-market trading price.
    /// The last trade price from the pre-market trading session.
    /// </summary>
    [JsonPropertyName("preMarket")]
    public decimal? PreMarket { get; set; }
}
