// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.Serialization;

namespace TreyThomasCodes.Polygon.Models.Common;

/// <summary>
/// Represents the sort order for query results.
/// </summary>
public enum SortOrder
{
    /// <summary>
    /// Ascending order (oldest to newest, A to Z, lowest to highest)
    /// </summary>
    [EnumMember(Value = "asc")]
    Ascending,

    /// <summary>
    /// Descending order (newest to oldest, Z to A, highest to lowest)
    /// </summary>
    [EnumMember(Value = "desc")]
    Descending
}
