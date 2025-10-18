// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

/// <summary>
/// Request object for retrieving tick-level trade data for a specific stock ticker symbol.
/// </summary>
public class GetTradesRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol for which to retrieve trade data (e.g., "AAPL", "MSFT").
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exact timestamp for querying specific trades (nanosecond precision).
    /// </summary>
    public string? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the filter for trades with timestamps greater than or equal to this value.
    /// </summary>
    public string? TimestampGte { get; set; }

    /// <summary>
    /// Gets or sets the filter for trades with timestamps greater than this value.
    /// </summary>
    public string? TimestampGt { get; set; }

    /// <summary>
    /// Gets or sets the filter for trades with timestamps less than or equal to this value.
    /// </summary>
    public string? TimestampLte { get; set; }

    /// <summary>
    /// Gets or sets the filter for trades with timestamps less than this value.
    /// </summary>
    public string? TimestampLt { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of trades to return (maximum varies by plan).
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the sort order for results (e.g., "timestamp", "timestamp.asc", "timestamp.desc").
    /// </summary>
    public string? Sort { get; set; }
}
