// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TreyThomasCodes.Polygon.Models.Options;

/// <summary>
/// Represents a comprehensive snapshot of current market data for an options contract.
/// Contains the most recent trade, quote, daily aggregate data, Greeks, and underlying asset information.
/// </summary>
public class OptionSnapshot
{
    /// <summary>
    /// Gets or sets the break-even price for this options contract.
    /// The price at which the underlying asset needs to trade at expiration for the option holder to break even (excluding commissions).
    /// For calls: strike price + premium paid. For puts: strike price - premium paid.
    /// </summary>
    [JsonPropertyName("break_even_price")]
    public decimal? BreakEvenPrice { get; set; }

    /// <summary>
    /// Gets or sets the daily aggregate data for the current trading session.
    /// Contains OHLC (Open, High, Low, Close), volume, and other metrics for the day.
    /// </summary>
    [JsonPropertyName("day")]
    public OptionDayData? Day { get; set; }

    /// <summary>
    /// Gets or sets the contract details for this options contract.
    /// Contains information about the contract type, strike price, expiration date, and other specifications.
    /// </summary>
    [JsonPropertyName("details")]
    public OptionContractDetails? Details { get; set; }

    /// <summary>
    /// Gets or sets the option Greeks for this contract.
    /// Greeks measure the sensitivity of the option's price to various factors (underlying price, time, volatility, etc.).
    /// </summary>
    [JsonPropertyName("greeks")]
    public OptionGreeks? Greeks { get; set; }

    /// <summary>
    /// Gets or sets the implied volatility for this options contract.
    /// Represents the market's forecast of a likely movement in the underlying asset's price, expressed as a decimal (e.g., 0.20 = 20%).
    /// </summary>
    [JsonPropertyName("implied_volatility")]
    public decimal? ImpliedVolatility { get; set; }

    /// <summary>
    /// Gets or sets the most recent quote data for this options contract.
    /// Contains the latest bid and ask prices, sizes, and exchange information.
    /// </summary>
    [JsonPropertyName("last_quote")]
    public OptionLastQuote? LastQuote { get; set; }

    /// <summary>
    /// Gets or sets the most recent trade data for this options contract.
    /// Contains information about the last executed trade.
    /// </summary>
    [JsonPropertyName("last_trade")]
    public OptionLastTrade? LastTrade { get; set; }

    /// <summary>
    /// Gets or sets the open interest for this options contract.
    /// Represents the total number of outstanding option contracts that have not been closed or exercised.
    /// </summary>
    [JsonPropertyName("open_interest")]
    public int? OpenInterest { get; set; }

    /// <summary>
    /// Gets or sets the underlying asset information.
    /// Contains current price and other data about the stock or asset that this option is based on.
    /// </summary>
    [JsonPropertyName("underlying_asset")]
    public OptionUnderlyingAsset? UnderlyingAsset { get; set; }
}

/// <summary>
/// Represents daily aggregate trading data for an options contract.
/// Contains OHLC (Open, High, Low, Close), volume, and other metrics for a trading day.
/// </summary>
public class OptionDayData
{
    /// <summary>
    /// Gets or sets the change in price from the previous close to the current close.
    /// Represents the absolute price change in currency units.
    /// </summary>
    [JsonPropertyName("change")]
    public decimal? Change { get; set; }

    /// <summary>
    /// Gets or sets the percentage change from the previous close to the current close.
    /// Represents the price change as a percentage.
    /// </summary>
    [JsonPropertyName("change_percent")]
    public decimal? ChangePercent { get; set; }

    /// <summary>
    /// Gets or sets the closing price for the trading day.
    /// The last trade price at the end of the regular trading session.
    /// </summary>
    [JsonPropertyName("close")]
    public decimal? Close { get; set; }

    /// <summary>
    /// Gets or sets the highest price reached during the trading day.
    /// The maximum trade price within the trading session.
    /// </summary>
    [JsonPropertyName("high")]
    public decimal? High { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this daily data was last updated.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("last_updated")]
    public long? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets the lowest price reached during the trading day.
    /// The minimum trade price within the trading session.
    /// </summary>
    [JsonPropertyName("low")]
    public decimal? Low { get; set; }

    /// <summary>
    /// Gets or sets the opening price for the trading day.
    /// The first trade price at the beginning of the regular trading session.
    /// </summary>
    [JsonPropertyName("open")]
    public decimal? Open { get; set; }

    /// <summary>
    /// Gets or sets the previous trading day's closing price.
    /// Used to calculate change and change_percent values.
    /// </summary>
    [JsonPropertyName("previous_close")]
    public decimal? PreviousClose { get; set; }

    /// <summary>
    /// Gets or sets the total trading volume for the day.
    /// The total number of contracts traded during the trading session.
    /// </summary>
    [JsonPropertyName("volume")]
    public long? Volume { get; set; }

    /// <summary>
    /// Gets or sets the volume-weighted average price (VWAP) for the trading day.
    /// The average price weighted by the volume of each trade during the session.
    /// </summary>
    [JsonPropertyName("vwap")]
    public decimal? Vwap { get; set; }
}

/// <summary>
/// Represents the contract details for an options contract in a snapshot.
/// Contains information about the contract type, strike price, expiration date, and other specifications.
/// </summary>
public class OptionContractDetails
{
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
}

/// <summary>
/// Represents the Greek values for an options contract.
/// Greeks measure the sensitivity of the option's price to various factors such as underlying price, time, and volatility.
/// </summary>
public class OptionGreeks
{
    /// <summary>
    /// Gets or sets the delta of the option.
    /// Measures the rate of change of the option's price with respect to changes in the underlying asset's price.
    /// For calls: ranges from 0 to 1. For puts: ranges from -1 to 0.
    /// </summary>
    [JsonPropertyName("delta")]
    public decimal? Delta { get; set; }

    /// <summary>
    /// Gets or sets the gamma of the option.
    /// Measures the rate of change of delta with respect to changes in the underlying asset's price.
    /// Gamma is highest for at-the-money options and decreases as options move in or out of the money.
    /// </summary>
    [JsonPropertyName("gamma")]
    public decimal? Gamma { get; set; }

    /// <summary>
    /// Gets or sets the theta of the option.
    /// Measures the rate of change of the option's price with respect to the passage of time (time decay).
    /// Usually negative for long options, indicating that options lose value as time passes.
    /// </summary>
    [JsonPropertyName("theta")]
    public decimal? Theta { get; set; }

    /// <summary>
    /// Gets or sets the vega of the option.
    /// Measures the sensitivity of the option's price to changes in the implied volatility of the underlying asset.
    /// Higher vega means the option's price is more sensitive to changes in volatility.
    /// </summary>
    [JsonPropertyName("vega")]
    public decimal? Vega { get; set; }
}

/// <summary>
/// Represents the most recent quote data for an options contract.
/// Contains the latest bid and ask prices, sizes, and exchange information.
/// </summary>
public class OptionLastQuote
{
    /// <summary>
    /// Gets or sets the ask price.
    /// The lowest price a seller is willing to accept for the option contract.
    /// </summary>
    [JsonPropertyName("ask")]
    public decimal? Ask { get; set; }

    /// <summary>
    /// Gets or sets the ask size.
    /// The number of contracts being offered at the ask price.
    /// </summary>
    [JsonPropertyName("ask_size")]
    public int? AskSize { get; set; }

    /// <summary>
    /// Gets or sets the ask exchange.
    /// Represented as a numeric code corresponding to the specific exchange offering the ask.
    /// </summary>
    [JsonPropertyName("ask_exchange")]
    public int? AskExchange { get; set; }

    /// <summary>
    /// Gets or sets the bid price.
    /// The highest price a buyer is willing to pay for the option contract.
    /// </summary>
    [JsonPropertyName("bid")]
    public decimal? Bid { get; set; }

    /// <summary>
    /// Gets or sets the bid size.
    /// The number of contracts being bid for at the bid price.
    /// </summary>
    [JsonPropertyName("bid_size")]
    public int? BidSize { get; set; }

    /// <summary>
    /// Gets or sets the bid exchange.
    /// Represented as a numeric code corresponding to the specific exchange offering the bid.
    /// </summary>
    [JsonPropertyName("bid_exchange")]
    public int? BidExchange { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this quote was last updated.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("last_updated")]
    public long? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets the midpoint between the bid and ask prices.
    /// Calculated as (bid + ask) / 2 and represents the theoretical fair value based on current quotes.
    /// </summary>
    [JsonPropertyName("midpoint")]
    public decimal? Midpoint { get; set; }

    /// <summary>
    /// Gets or sets the timeframe for this quote data.
    /// Indicates whether the data is from the real-time feed or delayed (e.g., "REAL-TIME", "DELAYED").
    /// </summary>
    [JsonPropertyName("timeframe")]
    public string? Timeframe { get; set; }
}

/// <summary>
/// Represents the most recent trade data for an options contract.
/// Contains information about the last executed trade including price, size, and conditions.
/// </summary>
public class OptionLastTrade
{
    /// <summary>
    /// Gets or sets the SIP (Securities Information Processor) timestamp when this trade was executed.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("sip_timestamp")]
    public long? SipTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the list of condition codes that apply to this trade.
    /// These codes provide additional context about the nature of the trade.
    /// </summary>
    [JsonPropertyName("conditions")]
    public List<int>? Conditions { get; set; }

    /// <summary>
    /// Gets or sets the price at which this trade was executed.
    /// The per-contract price in the option's trading currency.
    /// </summary>
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Gets or sets the number of contracts traded in this transaction.
    /// The volume or quantity of option contracts that changed hands.
    /// </summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }

    /// <summary>
    /// Gets or sets the exchange where this trade was executed.
    /// Represented as a numeric code corresponding to specific exchanges.
    /// </summary>
    [JsonPropertyName("exchange")]
    public int? Exchange { get; set; }

    /// <summary>
    /// Gets or sets the timeframe for this trade data.
    /// Indicates whether the data is from the real-time feed or delayed (e.g., "REAL-TIME", "DELAYED").
    /// </summary>
    [JsonPropertyName("timeframe")]
    public string? Timeframe { get; set; }
}

/// <summary>
/// Represents information about the underlying asset for an options contract.
/// Contains current price, ticker, and other data about the stock or asset that this option is based on.
/// </summary>
public class OptionUnderlyingAsset
{
    /// <summary>
    /// Gets or sets the change to break-even price.
    /// The amount by which the underlying asset's price must change to reach the option's break-even point.
    /// </summary>
    [JsonPropertyName("change_to_break_even")]
    public decimal? ChangeToBreakEven { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the underlying asset's price was last updated.
    /// Expressed as nanoseconds since the Unix epoch (January 1, 1970 UTC).
    /// </summary>
    [JsonPropertyName("last_updated")]
    public long? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets the current price of the underlying asset.
    /// The most recent trade price or market value of the stock or asset that this option is based on.
    /// </summary>
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Gets or sets the ticker symbol of the underlying asset.
    /// For example, "SPY" for SPDR S&amp;P 500 ETF Trust, "AAPL" for Apple Inc.
    /// </summary>
    [JsonPropertyName("ticker")]
    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the timeframe for the underlying asset data.
    /// Indicates whether the data is from the real-time feed or delayed (e.g., "REAL-TIME", "DELAYED").
    /// </summary>
    [JsonPropertyName("timeframe")]
    public string? Timeframe { get; set; }
}
