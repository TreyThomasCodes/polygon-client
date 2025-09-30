// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Configuration;

/// <summary>
/// Configuration options for the Polygon.io API client.
/// Contains settings for authentication, connection parameters, and retry behavior.
/// </summary>
public class PolygonOptions
{
    /// <summary>
    /// The configuration section name used in appsettings.json or other configuration sources.
    /// </summary>
    public const string SectionName = "Polygon";

    /// <summary>
    /// Gets or sets the Polygon.io API key for authentication.
    /// This is required to access the Polygon.io APIs and can be obtained from the Polygon.io dashboard.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL for the Polygon.io API.
    /// Defaults to "https://api.polygon.io" for production use.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.polygon.io";

    /// <summary>
    /// Gets or sets the timeout duration for HTTP requests to the Polygon.io API.
    /// Defaults to 30 seconds. Increase for slow connections or large data requests.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for failed API requests.
    /// Defaults to 3 retries. Set to 0 to disable retries.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the delay between retry attempts for failed API requests.
    /// Defaults to 1 second. Increase to reduce API load during retries.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}