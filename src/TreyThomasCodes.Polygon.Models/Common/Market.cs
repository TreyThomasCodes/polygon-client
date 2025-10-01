// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.Serialization;

namespace TreyThomasCodes.Polygon.Models.Common;

/// <summary>
/// Represents the market type for ticker filtering.
/// </summary>
public enum Market
{
    /// <summary>
    /// Stock market (equities)
    /// </summary>
    [EnumMember(Value = "stocks")]
    Stocks,

    /// <summary>
    /// Cryptocurrency market
    /// </summary>
    [EnumMember(Value = "crypto")]
    Crypto,

    /// <summary>
    /// Foreign exchange (forex) market
    /// </summary>
    [EnumMember(Value = "fx")]
    Forex
}
