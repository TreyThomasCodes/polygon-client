// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Net;
using Refit;

namespace TreyThomasCodes.Polygon.RestClient.Exceptions;

/// <summary>
/// Exception thrown when the Polygon.io API returns an error response.
/// </summary>
/// <remarks>
/// This exception is thrown when an API call fails due to an HTTP error response from the Polygon.io API.
/// It wraps Refit's <see cref="ApiException"/> and provides easy access to the HTTP status code, response content,
/// and request URI without exposing Refit types to library consumers.
/// <para>
/// The original <see cref="ApiException"/> is available through the <see cref="Exception.InnerException"/> property.
/// </para>
/// <para>
/// Common HTTP status codes and their meanings:
/// <list type="bullet">
/// <item><description>401 Unauthorized - Invalid or missing API key</description></item>
/// <item><description>403 Forbidden - Insufficient permissions for the requested data</description></item>
/// <item><description>404 Not Found - Invalid ticker symbol or endpoint</description></item>
/// <item><description>429 Too Many Requests - API rate limit exceeded</description></item>
/// <item><description>500 Internal Server Error - Polygon.io API error</description></item>
/// </list>
/// </para>
/// </remarks>
public class PolygonApiException : PolygonException
{
    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the raw response content returned by the API, or null if not available.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Gets the URI of the failed request.
    /// </summary>
    public Uri RequestUri { get; }

    /// <summary>
    /// Gets a value indicating whether this error represents an unauthorized (401) response.
    /// </summary>
    public bool IsUnauthorized => StatusCode == HttpStatusCode.Unauthorized;

    /// <summary>
    /// Gets a value indicating whether this error represents a forbidden (403) response.
    /// </summary>
    public bool IsForbidden => StatusCode == HttpStatusCode.Forbidden;

    /// <summary>
    /// Gets a value indicating whether this error represents a not found (404) response.
    /// </summary>
    public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;

    /// <summary>
    /// Gets a value indicating whether this error represents a rate limit (429) response.
    /// </summary>
    public bool IsRateLimited => StatusCode == HttpStatusCode.TooManyRequests;

    /// <summary>
    /// Gets a value indicating whether this error represents a server error (5xx) response.
    /// </summary>
    public bool IsServerError => (int)StatusCode >= 500 && (int)StatusCode < 600;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonApiException"/> class.
    /// </summary>
    /// <param name="apiException">The Refit ApiException to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="apiException"/> is null.</exception>
    public PolygonApiException(ApiException apiException)
        : base(BuildErrorMessage(apiException), apiException)
    {
        ArgumentNullException.ThrowIfNull(apiException);

        StatusCode = apiException.StatusCode;
        ResponseContent = apiException.Content;
        RequestUri = apiException.Uri ?? throw new ArgumentException("ApiException.Uri cannot be null", nameof(apiException));
    }

    /// <summary>
    /// Builds a descriptive error message based on the API exception details.
    /// </summary>
    /// <param name="apiException">The API exception containing error details.</param>
    /// <returns>A formatted error message.</returns>
    private static string BuildErrorMessage(ApiException apiException)
    {
        ArgumentNullException.ThrowIfNull(apiException);

        var statusCode = apiException.StatusCode;
        var reasonPhrase = apiException.ReasonPhrase ?? "Unknown error";
        var uri = apiException.Uri?.PathAndQuery ?? "unknown endpoint";

        return statusCode switch
        {
            HttpStatusCode.Unauthorized => $"API authentication failed (401 Unauthorized). Please verify your API key is valid. Endpoint: {uri}",
            HttpStatusCode.Forbidden => $"API access forbidden (403 Forbidden). Your API key may not have permission to access this data. Endpoint: {uri}",
            HttpStatusCode.NotFound => $"API resource not found (404 Not Found). The requested ticker or endpoint may be invalid. Endpoint: {uri}",
            HttpStatusCode.TooManyRequests => $"API rate limit exceeded (429 Too Many Requests). Please reduce request frequency. Endpoint: {uri}",
            _ when (int)statusCode >= 500 => $"Polygon.io API server error ({(int)statusCode} {reasonPhrase}). Endpoint: {uri}",
            _ => $"Polygon.io API error ({(int)statusCode} {reasonPhrase}). Endpoint: {uri}"
        };
    }
}
