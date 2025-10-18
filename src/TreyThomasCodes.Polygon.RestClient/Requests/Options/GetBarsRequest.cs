// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;

namespace TreyThomasCodes.Polygon.RestClient.Requests.Options;

/// <summary>
/// Request object for retrieving aggregate OHLC (bar/candle) data for an options contract over a specified time range.
/// </summary>
public class GetBarsRequest
{
    /// <summary>
    /// Gets or sets the options ticker symbol in OCC format (e.g., "O:SPY251219C00650000").
    /// The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.
    /// </summary>
    public string OptionsTicker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of timespan units to aggregate (e.g., 1 for 1 day, 5 for 5 minutes, 15 for 15 minutes).
    /// </summary>
    public int Multiplier { get; set; }

    /// <summary>
    /// Gets or sets the size of the time window for each aggregate (e.g., minute, hour, day, week, month, quarter, year).
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
    /// Gets or sets whether to adjust for splits. Defaults to true if not specified.
    /// Note that options contracts are not adjusted for underlying stock splits.
    /// </summary>
    public bool? Adjusted { get; set; }

    /// <summary>
    /// Gets or sets the sort order for results (asc for ascending, desc for descending by timestamp).
    /// </summary>
    public SortOrder? Sort { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of aggregate results to return. Maximum value varies by plan.
    /// </summary>
    public int? Limit { get; set; }
}
