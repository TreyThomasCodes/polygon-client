// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;

namespace TreyThomasCodes.Polygon.RestClient.Requests.Reference;

/// <summary>
/// Request object for retrieving a comprehensive list of ticker symbols supported by Polygon.io.
/// Provides detailed company information and supports extensive filtering and searching capabilities.
/// </summary>
public class GetTickersRequest
{
    /// <summary>
    /// Gets or sets the filter for exact ticker symbol (e.g., "AAPL").
    /// </summary>
    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the filter for tickers alphabetically greater than this value.
    /// </summary>
    public string? TickerGt { get; set; }

    /// <summary>
    /// Gets or sets the filter for tickers alphabetically greater than or equal to this value.
    /// </summary>
    public string? TickerGte { get; set; }

    /// <summary>
    /// Gets or sets the filter for tickers alphabetically less than this value.
    /// </summary>
    public string? TickerLt { get; set; }

    /// <summary>
    /// Gets or sets the filter for tickers alphabetically less than or equal to this value.
    /// </summary>
    public string? TickerLte { get; set; }

    /// <summary>
    /// Gets or sets the filter by security type (e.g., "CS" for Common Stock, "ETF" for Exchange Traded Fund).
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the filter by market type.
    /// </summary>
    public Market? Market { get; set; }

    /// <summary>
    /// Gets or sets the filter by primary exchange (e.g., "XNYS", "XNAS").
    /// </summary>
    public string? Exchange { get; set; }

    /// <summary>
    /// Gets or sets the filter by CUSIP identifier.
    /// </summary>
    public string? Cusip { get; set; }

    /// <summary>
    /// Gets or sets the filter by SEC Central Index Key (CIK).
    /// </summary>
    public string? Cik { get; set; }

    /// <summary>
    /// Gets or sets the filter for tickers active on this date in YYYY-MM-DD format.
    /// </summary>
    public string? Date { get; set; }

    /// <summary>
    /// Gets or sets the search term for tickers by name or ticker symbol.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets or sets the filter by active status (true for active tickers, false for inactive).
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// Gets or sets the field to sort by.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    public SortOrder? Order { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of results to return (maximum 1000).
    /// </summary>
    public int? Limit { get; set; }
}
