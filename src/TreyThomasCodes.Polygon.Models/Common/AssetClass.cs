// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.Serialization;

namespace TreyThomasCodes.Polygon.Models.Common;

/// <summary>
/// Represents the asset class for filtering conditions, exchanges, and other reference data.
/// </summary>
public enum AssetClass
{
    /// <summary>
    /// Stock/equity securities
    /// </summary>
    [EnumMember(Value = "stocks")]
    Stocks,

    /// <summary>
    /// Options contracts
    /// </summary>
    [EnumMember(Value = "options")]
    Options,

    /// <summary>
    /// Cryptocurrency assets
    /// </summary>
    [EnumMember(Value = "crypto")]
    Crypto,

    /// <summary>
    /// Foreign exchange (forex) currency pairs
    /// </summary>
    [EnumMember(Value = "fx")]
    Forex
}
