// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.Models.Options;

/// <summary>
/// Represents the type of an options contract.
/// </summary>
public enum OptionType
{
    /// <summary>
    /// A call option gives the holder the right, but not the obligation, to buy the underlying asset at the strike price.
    /// Call options increase in value when the underlying asset price rises.
    /// </summary>
    Call,

    /// <summary>
    /// A put option gives the holder the right, but not the obligation, to sell the underlying asset at the strike price.
    /// Put options increase in value when the underlying asset price falls.
    /// </summary>
    Put
}
