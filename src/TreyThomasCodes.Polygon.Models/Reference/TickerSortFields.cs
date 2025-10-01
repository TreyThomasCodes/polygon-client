// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.Models.Reference;

/// <summary>
/// Provides constant values for valid sort fields when querying tickers.
/// </summary>
public static class TickerSortFields
{
    /// <summary>
    /// Sort by ticker symbol
    /// </summary>
    public const string Ticker = "ticker";

    /// <summary>
    /// Sort by company/security name
    /// </summary>
    public const string Name = "name";

    /// <summary>
    /// Sort by market type
    /// </summary>
    public const string Market = "market";

    /// <summary>
    /// Sort by locale
    /// </summary>
    public const string Locale = "locale";

    /// <summary>
    /// Sort by primary exchange
    /// </summary>
    public const string PrimaryExchange = "primary_exchange";

    /// <summary>
    /// Sort by security type
    /// </summary>
    public const string Type = "type";

    /// <summary>
    /// Sort by active status
    /// </summary>
    public const string Active = "active";

    /// <summary>
    /// Sort by currency symbol
    /// </summary>
    public const string CurrencySymbol = "currency_symbol";

    /// <summary>
    /// Sort by Central Index Key (CIK)
    /// </summary>
    public const string Cik = "cik";

    /// <summary>
    /// Sort by composite FIGI identifier
    /// </summary>
    public const string CompositeFigi = "composite_figi";

    /// <summary>
    /// Sort by share class FIGI identifier
    /// </summary>
    public const string ShareClassFigi = "share_class_figi";

    /// <summary>
    /// Sort by last updated timestamp (UTC)
    /// </summary>
    public const string LastUpdatedUtc = "last_updated_utc";
}
