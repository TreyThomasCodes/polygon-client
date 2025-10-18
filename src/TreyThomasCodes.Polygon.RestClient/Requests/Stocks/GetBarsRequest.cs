// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;

namespace TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

/// <summary>
/// Request object for retrieving aggregate OHLC data for a stock ticker over a specified time range.
/// </summary>
public class GetBarsRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol for which to retrieve aggregate data (e.g., "AAPL", "MSFT").
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of timespan units to aggregate (e.g., 1 for 1 day, 5 for 5 minutes).
    /// </summary>
    public int Multiplier { get; set; }

    /// <summary>
    /// Gets or sets the size of the time window for each aggregate.
    /// </summary>
    public AggregateInterval Timespan { get; set; }

    /// <summary>
    /// Gets or sets the start date for the aggregate window in YYYY-MM-DD format.
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the end date for the aggregate window in YYYY-MM-DD format.
    /// </summary>
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to adjust for stock splits and dividend payments. Defaults to true if not specified.
    /// </summary>
    public bool? Adjusted { get; set; }

    /// <summary>
    /// Gets or sets the sort order for results.
    /// </summary>
    public SortOrder? Sort { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of aggregate results to return (maximum 50,000).
    /// </summary>
    public int? Limit { get; set; }
}
