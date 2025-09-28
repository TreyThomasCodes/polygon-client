// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace TTC.Polygon.Models.Common;

/// <summary>
/// Represents a standard response structure from the Polygon.io API.
/// Contains metadata about the request and the actual results data.
/// </summary>
/// <typeparam name="T">The type of the results data returned by the API.</typeparam>
public class PolygonResponse<T>
{
    /// <summary>
    /// Gets or sets the ticker symbol for the request.
    /// Represents the stock ticker that was queried.
    /// </summary>
    [JsonPropertyName("ticker")]
    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the number of queries made to retrieve this data.
    /// Indicates how many individual queries were executed by the API.
    /// </summary>
    [JsonPropertyName("queryCount")]
    public int? QueryCount { get; set; }

    /// <summary>
    /// Gets or sets the number of results returned in this response.
    /// Indicates the actual count of result items returned.
    /// </summary>
    [JsonPropertyName("resultsCount")]
    public int? ResultsCount { get; set; }

    /// <summary>
    /// Gets or sets whether the results have been adjusted for splits and dividends.
    /// True if the data has been adjusted, false otherwise.
    /// </summary>
    [JsonPropertyName("adjusted")]
    public bool? Adjusted { get; set; }

    /// <summary>
    /// Gets or sets the actual data results from the API call.
    /// The type varies depending on the specific API endpoint called.
    /// </summary>
    [JsonPropertyName("results")]
    public T? Results { get; set; }

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

    /// <summary>
    /// Gets or sets the number of results returned in this response.
    /// May be null if the endpoint doesn't provide count information.
    /// </summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>
    /// Gets or sets the URL for retrieving the next page of results.
    /// Used for pagination when there are more results available than returned in a single response.
    /// </summary>
    [JsonPropertyName("next_url")]
    public string? NextUrl { get; set; }
}

/// <summary>
/// Represents an error response structure from the Polygon.io API.
/// Contains details about what went wrong with the API request.
/// </summary>
public class PolygonErrorResponse
{
    /// <summary>
    /// Gets or sets the error status of the API response.
    /// Typically "ERROR" for failed requests.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error code or type.
    /// Provides a machine-readable indication of the error category.
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable error message.
    /// Provides details about what caused the error and how to resolve it.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique request identifier assigned by Polygon.io.
    /// Useful for debugging and support requests when errors occur.
    /// </summary>
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;
}