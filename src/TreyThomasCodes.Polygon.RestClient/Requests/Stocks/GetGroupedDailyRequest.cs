// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

/// <summary>
/// Request object for retrieving the daily OHLC aggregate data for all US stock tickers on a specific date.
/// </summary>
public class GetGroupedDailyRequest
{
    /// <summary>
    /// Gets or sets the date for which to retrieve grouped daily aggregates in YYYY-MM-DD format.
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to adjust for stock splits and dividend payments. Defaults to true if not specified.
    /// </summary>
    public bool? Adjusted { get; set; }

    /// <summary>
    /// Gets or sets whether to include over-the-counter (OTC) securities in the results. Defaults to false if not specified.
    /// </summary>
    public bool? IncludeOtc { get; set; }
}
