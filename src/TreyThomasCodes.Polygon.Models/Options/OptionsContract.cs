// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TreyThomasCodes.Polygon.Models.Options;

/// <summary>
/// Represents detailed information about an options contract including strike price, expiration date, and underlying asset.
/// Contains comprehensive metadata about an options contract's terms, style, and trading characteristics.
/// </summary>
public class OptionsContract
{
    /// <summary>
    /// Gets or sets the Classification of Financial Instruments (CFI) code for this options contract.
    /// A six-character code that describes the financial instrument's category and attributes according to ISO 10962 standards.
    /// For example, "OCASPS" indicates a Call Option, American style, with Physical settlement.
    /// </summary>
    [JsonPropertyName("cfi")]
    public string? Cfi { get; set; }

    /// <summary>
    /// Gets or sets the type of options contract.
    /// Common values include "call" for call options and "put" for put options.
    /// </summary>
    [JsonPropertyName("contract_type")]
    public string? ContractType { get; set; }

    /// <summary>
    /// Gets or sets the exercise style of the options contract.
    /// Common values include "american" (can be exercised any time before expiration) and "european" (can only be exercised at expiration).
    /// </summary>
    [JsonPropertyName("exercise_style")]
    public string? ExerciseStyle { get; set; }

    /// <summary>
    /// Gets or sets the expiration date of the options contract in YYYY-MM-DD format.
    /// This is the last date on which the option can be exercised or will expire worthless.
    /// </summary>
    [JsonPropertyName("expiration_date")]
    public string? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the primary exchange where this options contract is listed and traded.
    /// Examples include "BATO" (Cboe BZX Exchange), "AMEX" (NYSE American), "CBOE" (Cboe Options Exchange).
    /// </summary>
    [JsonPropertyName("primary_exchange")]
    public string? PrimaryExchange { get; set; }

    /// <summary>
    /// Gets or sets the number of shares of the underlying asset that this options contract controls.
    /// Typically 100 for standard equity options contracts.
    /// </summary>
    [JsonPropertyName("shares_per_contract")]
    public int? SharesPerContract { get; set; }

    /// <summary>
    /// Gets or sets the strike price (also known as exercise price) of the options contract.
    /// This is the predetermined price at which the underlying asset can be bought (for calls) or sold (for puts) when the option is exercised.
    /// </summary>
    [JsonPropertyName("strike_price")]
    public decimal? StrikePrice { get; set; }

    /// <summary>
    /// Gets or sets the ticker symbol for this specific options contract.
    /// Follows the OCC (Options Clearing Corporation) format, for example: "O:SPY251219C00650000"
    /// where O: indicates options, SPY is the underlying, 251219 is the expiration date (YYMMDD), C is call/put indicator, and the rest is the strike price.
    /// </summary>
    [JsonPropertyName("ticker")]
    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the ticker symbol of the underlying asset that this options contract is based on.
    /// For example, "SPY" for SPDR S&amp;P 500 ETF Trust, "AAPL" for Apple Inc.
    /// </summary>
    [JsonPropertyName("underlying_ticker")]
    public string? UnderlyingTicker { get; set; }
}
