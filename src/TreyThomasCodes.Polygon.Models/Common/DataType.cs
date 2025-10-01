// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.Serialization;

namespace TreyThomasCodes.Polygon.Models.Common;

/// <summary>
/// Represents the data type for filtering condition codes.
/// </summary>
public enum DataType
{
    /// <summary>
    /// Trade data
    /// </summary>
    [EnumMember(Value = "trade")]
    Trade,

    /// <summary>
    /// Quote data
    /// </summary>
    [EnumMember(Value = "quote")]
    Quote
}
