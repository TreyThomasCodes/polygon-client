// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TreyThomasCodes.Polygon.Models.Reference;

/// <summary>
/// Represents the response from the Polygon.io ticker types endpoint.
/// Contains a list of ticker types along with response metadata.
/// </summary>
public class TickerTypesResponse
{
    /// <summary>
    /// Gets or sets the list of ticker types returned by the API.
    /// Contains information about different security types available in the market.
    /// </summary>
    [JsonPropertyName("results")]
    public List<TickerType> Results { get; set; } = new();

    /// <summary>
    /// Gets or sets the number of ticker types returned in this response.
    /// Indicates the actual count of result items.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the status of the API response.
    /// Typically "OK" for successful requests or "ERROR" for failed requests.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique request identifier assigned by Polygon.io.
    /// Useful for debugging and support requests.
    /// </summary>
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;
}