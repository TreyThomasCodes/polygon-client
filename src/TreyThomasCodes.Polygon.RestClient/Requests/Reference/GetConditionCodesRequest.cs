// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;

namespace TreyThomasCodes.Polygon.RestClient.Requests.Reference;

/// <summary>
/// Request object for retrieving a unified list of trade and quote conditions from various market data providers.
/// </summary>
public class GetConditionCodesRequest
{
    /// <summary>
    /// Gets or sets the filter for conditions by asset class.
    /// </summary>
    public AssetClass? AssetClass { get; set; }

    /// <summary>
    /// Gets or sets the filter for conditions by data type.
    /// </summary>
    public DataType? DataType { get; set; }

    /// <summary>
    /// Gets or sets the filter by condition ID. Can be a single ID or comma-separated list of IDs.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the filter by SIP mapping type.
    /// </summary>
    public SipMappingType? SipMapping { get; set; }

    /// <summary>
    /// Gets or sets the sort order for the results.
    /// </summary>
    public SortOrder? Order { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of results to return. Maximum value is 1000. Default is 10.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the field to sort by. Default is "asset_class".
    /// </summary>
    public string? Sort { get; set; }
}
