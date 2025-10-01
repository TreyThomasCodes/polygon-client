// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.Serialization;

namespace TreyThomasCodes.Polygon.Models.Common;

/// <summary>
/// Represents the time interval unit for aggregate data windows.
/// Used in combination with a multiplier to specify the aggregation period (e.g., 5 minutes, 1 day).
/// </summary>
public enum AggregateInterval
{
    /// <summary>
    /// Minute-level aggregates
    /// </summary>
    [EnumMember(Value = "minute")]
    Minute,

    /// <summary>
    /// Hour-level aggregates
    /// </summary>
    [EnumMember(Value = "hour")]
    Hour,

    /// <summary>
    /// Day-level aggregates
    /// </summary>
    [EnumMember(Value = "day")]
    Day,

    /// <summary>
    /// Week-level aggregates
    /// </summary>
    [EnumMember(Value = "week")]
    Week,

    /// <summary>
    /// Month-level aggregates
    /// </summary>
    [EnumMember(Value = "month")]
    Month,

    /// <summary>
    /// Quarter-level aggregates
    /// </summary>
    [EnumMember(Value = "quarter")]
    Quarter,

    /// <summary>
    /// Year-level aggregates
    /// </summary>
    [EnumMember(Value = "year")]
    Year
}
