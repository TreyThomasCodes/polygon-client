// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Exceptions;

/// <summary>
/// The base exception class for all Polygon.io API related exceptions.
/// </summary>
/// <remarks>
/// This exception serves as the base class for all custom exceptions thrown by the TreyThomasCodes.Polygon library.
/// Catching this exception type will catch all Polygon-specific errors, allowing consumers to handle all library
/// exceptions with a single catch block if desired.
/// </remarks>
public class PolygonException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonException"/> class.
    /// </summary>
    public PolygonException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PolygonException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public PolygonException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
