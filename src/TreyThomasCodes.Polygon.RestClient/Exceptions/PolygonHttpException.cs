// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Exceptions;

/// <summary>
/// Exception thrown when a network or HTTP transport error occurs while communicating with the Polygon.io API.
/// </summary>
/// <remarks>
/// This exception is thrown when an API call fails due to network connectivity issues, DNS resolution failures,
/// connection timeouts, SSL/TLS errors, or other HTTP transport layer problems. It wraps exceptions like
/// <see cref="HttpRequestException"/> and <see cref="TaskCanceledException"/> to provide a consistent
/// exception hierarchy for library consumers.
/// <para>
/// The original exception is available through the <see cref="Exception.InnerException"/> property.
/// </para>
/// <para>
/// Common scenarios that trigger this exception:
/// <list type="bullet">
/// <item><description>Network connectivity failures</description></item>
/// <item><description>DNS resolution failures</description></item>
/// <item><description>Connection timeouts</description></item>
/// <item><description>SSL/TLS certificate errors</description></item>
/// <item><description>Request timeouts (when not cancelled by user)</description></item>
/// </list>
/// </para>
/// </remarks>
public class PolygonHttpException : PolygonException
{
    /// <summary>
    /// Gets a value indicating whether this exception represents a timeout.
    /// </summary>
    public bool IsTimeout { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonHttpException"/> class.
    /// </summary>
    /// <param name="message">The error message that describes the network or HTTP error.</param>
    /// <param name="innerException">The underlying HTTP or network exception.</param>
    public PolygonHttpException(string message, Exception innerException)
        : base(message, innerException)
    {
        IsTimeout = innerException is TaskCanceledException or TimeoutException;
    }

    /// <summary>
    /// Creates a <see cref="PolygonHttpException"/> from an <see cref="HttpRequestException"/>.
    /// </summary>
    /// <param name="httpException">The HTTP request exception to wrap.</param>
    /// <returns>A new <see cref="PolygonHttpException"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpException"/> is null.</exception>
    public static PolygonHttpException FromHttpRequestException(HttpRequestException httpException)
    {
        ArgumentNullException.ThrowIfNull(httpException);

        var message = "Network error occurred while communicating with Polygon.io API. " +
                      "Please check your internet connection and try again. " +
                      $"Details: {httpException.Message}";

        return new PolygonHttpException(message, httpException);
    }

    /// <summary>
    /// Creates a <see cref="PolygonHttpException"/> from a <see cref="TaskCanceledException"/> representing a timeout.
    /// </summary>
    /// <param name="timeoutException">The task cancellation exception representing a timeout.</param>
    /// <returns>A new <see cref="PolygonHttpException"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="timeoutException"/> is null.</exception>
    public static PolygonHttpException FromTimeout(TaskCanceledException timeoutException)
    {
        ArgumentNullException.ThrowIfNull(timeoutException);

        var message = "Request to Polygon.io API timed out. " +
                      "The server may be experiencing high load or your connection may be slow. " +
                      "Please try again later or increase the timeout configuration.";

        return new PolygonHttpException(message, timeoutException);
    }
}
