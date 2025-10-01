// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.Serialization;

namespace TreyThomasCodes.Polygon.Models.Common;

/// <summary>
/// Represents the geographic locale for filtering exchanges and other reference data.
/// </summary>
public enum Locale
{
    /// <summary>
    /// United States exchanges and markets
    /// </summary>
    [EnumMember(Value = "us")]
    UnitedStates,

    /// <summary>
    /// Global/international exchanges and markets
    /// </summary>
    [EnumMember(Value = "global")]
    Global
}
