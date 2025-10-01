// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.Serialization;

namespace TreyThomasCodes.Polygon.Models.Common;

/// <summary>
/// Represents the Securities Information Processor (SIP) mapping type for filtering condition codes.
/// </summary>
public enum SipMappingType
{
    /// <summary>
    /// Consolidated Tape Association (CTA) - responsible for NYSE and other regional exchanges
    /// </summary>
    [EnumMember(Value = "CTA")]
    CTA,

    /// <summary>
    /// Unlisted Trading Privileges (UTP) - responsible for NASDAQ-listed securities
    /// </summary>
    [EnumMember(Value = "UTP")]
    UTP,

    /// <summary>
    /// FINRA Trade Data Dissemination Service - responsible for over-the-counter (OTC) trades
    /// </summary>
    [EnumMember(Value = "FINRA_TDDS")]
    FinraTdds
}
