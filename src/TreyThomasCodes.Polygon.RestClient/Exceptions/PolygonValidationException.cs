// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;

namespace TreyThomasCodes.Polygon.RestClient.Exceptions;

/// <summary>
/// Exception thrown when a request object fails validation.
/// </summary>
/// <remarks>
/// This exception is thrown when request parameters fail validation before an API call is made.
/// It wraps FluentValidation's <see cref="ValidationException"/> and provides structured access
/// to validation errors through the <see cref="Errors"/> property without exposing FluentValidation
/// types to library consumers.
/// <para>
/// The original <see cref="ValidationException"/> is available through the <see cref="Exception.InnerException"/> property.
/// </para>
/// </remarks>
public class PolygonValidationException : PolygonException
{
    /// <summary>
    /// Gets the collection of validation errors that occurred.
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonValidationException"/> class.
    /// </summary>
    /// <param name="validationException">The FluentValidation exception to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="validationException"/> is null.</exception>
    public PolygonValidationException(ValidationException validationException)
        : base(BuildErrorMessage(validationException), validationException)
    {
        ArgumentNullException.ThrowIfNull(validationException);

        Errors = validationException.Errors
            .Select(ValidationError.FromValidationFailure)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Builds a summary error message from the validation errors.
    /// </summary>
    /// <param name="validationException">The validation exception containing the errors.</param>
    /// <returns>A formatted error message summarizing all validation failures.</returns>
    private static string BuildErrorMessage(ValidationException validationException)
    {
        ArgumentNullException.ThrowIfNull(validationException);

        if (!validationException.Errors.Any())
        {
            return "Request validation failed.";
        }

        var errorCount = validationException.Errors.Count();
        var errorMessages = string.Join("; ", validationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

        return errorCount == 1
            ? $"Request validation failed: {errorMessages}"
            : $"Request validation failed with {errorCount} errors: {errorMessages}";
    }
}
