// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Requests.Options;

/// <summary>
/// Request object for retrieving a snapshot of current market data for all options contracts for a given underlying asset.
/// Supports pagination and filtering by expiration date, strike price, and contract type.
/// </summary>
public class GetChainSnapshotRequest
{
    /// <summary>
    /// Gets or sets the ticker symbol of the underlying asset (e.g., "SPY", "AAPL", "MSTR").
    /// </summary>
    public string UnderlyingAsset { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the strike price filter. Only returns contracts with this exact strike price.
    /// </summary>
    public decimal? StrikePrice { get; set; }

    /// <summary>
    /// Gets or sets the contract type filter. Use "call" for call options or "put" for put options.
    /// </summary>
    public string? ContractType { get; set; }

    /// <summary>
    /// Gets or sets the minimum expiration date filter in YYYY-MM-DD format.
    /// Only returns contracts expiring on or after this date.
    /// </summary>
    public string? ExpirationDateGte { get; set; }

    /// <summary>
    /// Gets or sets the maximum expiration date filter in YYYY-MM-DD format.
    /// Only returns contracts expiring on or before this date.
    /// </summary>
    public string? ExpirationDateLte { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of results to return. Maximum value varies by plan.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the sort order for results. Use "asc" for ascending or "desc" for descending.
    /// </summary>
    public string? Order { get; set; }

    /// <summary>
    /// Gets or sets the field to sort by (e.g., "ticker", "strike_price", "expiration_date").
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Gets or sets the cursor for pagination. Use the next_url from the previous response to get the next page of results.
    /// </summary>
    public string? Cursor { get; set; }
}
