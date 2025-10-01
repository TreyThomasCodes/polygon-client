// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.Models.Reference;

/// <summary>
/// Provides constant values for valid sort fields when querying condition codes.
/// </summary>
public static class ConditionCodeSortFields
{
    /// <summary>
    /// Sort by asset class
    /// </summary>
    public const string AssetClass = "asset_class";

    /// <summary>
    /// Sort by data type (trade or quote)
    /// </summary>
    public const string DataType = "data_type";

    /// <summary>
    /// Sort by condition ID
    /// </summary>
    public const string Id = "id";

    /// <summary>
    /// Sort by condition type
    /// </summary>
    public const string Type = "type";

    /// <summary>
    /// Sort by condition name
    /// </summary>
    public const string Name = "name";
}
