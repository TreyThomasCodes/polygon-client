// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Options;

/// <summary>
/// Request object for retrieving historical quotes (bid/ask) for a specific options contract.
/// Supports pagination and time-based filtering.
/// </summary>
public class GetQuotesRequest
{
    /// <summary>
    /// Gets or sets the options ticker symbol in OCC format (e.g., "O:SPY241220P00720000").
    /// The ticker must include the "O:" prefix followed by the underlying ticker, expiration date (YYMMDD), contract type (C for call, P for put), and strike price.
    /// </summary>
    public string OptionsTicker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp to query for quotes at or after this time.
    /// Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.
    /// </summary>
    public string? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the timestamp to query for quotes before this time.
    /// Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.
    /// </summary>
    public string? TimestampLt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp to query for quotes at or before this time.
    /// Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.
    /// </summary>
    public string? TimestampLte { get; set; }

    /// <summary>
    /// Gets or sets the timestamp to query for quotes after this time.
    /// Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.
    /// </summary>
    public string? TimestampGt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp to query for quotes at or after this time.
    /// Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.
    /// </summary>
    public string? TimestampGte { get; set; }

    /// <summary>
    /// Gets or sets the sort order for results. Use "asc" for ascending or "desc" for descending by timestamp.
    /// </summary>
    public string? Order { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of results to return. Maximum value varies by plan.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the sort field for results. Defaults to "timestamp".
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Gets or sets the cursor for pagination. Use the next_url from the previous response to get the next page of results.
    /// </summary>
    public string? Cursor { get; set; }
}
