// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using FluentValidation.Results;

namespace TreyThomasCodes.Polygon.RestClient.Exceptions;

/// <summary>
/// Represents a single validation error for a request parameter.
/// </summary>
/// <remarks>
/// This class provides validation error information without exposing FluentValidation types
/// to library consumers. It maps from <see cref="ValidationFailure"/> but provides a clean,
/// library-specific API surface.
/// </remarks>
public class ValidationError
{
    /// <summary>
    /// Gets the name of the property that failed validation.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the error message describing why the validation failed.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Gets the value that was attempted for the property, or null if not available.
    /// </summary>
    public object? AttemptedValue { get; }

    /// <summary>
    /// Gets the severity of the validation error.
    /// </summary>
    public ValidationSeverity Severity { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation.</param>
    /// <param name="errorMessage">The error message describing the validation failure.</param>
    /// <param name="attemptedValue">The value that was attempted for the property.</param>
    /// <param name="severity">The severity of the validation error.</param>
    public ValidationError(string propertyName, string errorMessage, object? attemptedValue, ValidationSeverity severity)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
        AttemptedValue = attemptedValue;
        Severity = severity;
    }

    /// <summary>
    /// Creates a <see cref="ValidationError"/> from a FluentValidation <see cref="ValidationFailure"/>.
    /// </summary>
    /// <param name="failure">The FluentValidation failure to convert.</param>
    /// <returns>A new <see cref="ValidationError"/> instance.</returns>
    internal static ValidationError FromValidationFailure(ValidationFailure failure)
    {
        ArgumentNullException.ThrowIfNull(failure);

        // Convert FluentValidation.Severity to our ValidationSeverity enum
        var severity = failure.Severity switch
        {
            FluentValidation.Severity.Error => ValidationSeverity.Error,
            FluentValidation.Severity.Warning => ValidationSeverity.Warning,
            FluentValidation.Severity.Info => ValidationSeverity.Info,
            _ => ValidationSeverity.Error
        };

        return new ValidationError(
            failure.PropertyName,
            failure.ErrorMessage,
            failure.AttemptedValue,
            severity
        );
    }

    /// <summary>
    /// Returns a string representation of the validation error.
    /// </summary>
    /// <returns>A string in the format "PropertyName: ErrorMessage".</returns>
    public override string ToString()
    {
        return $"{PropertyName}: {ErrorMessage}";
    }
}

/// <summary>
/// Defines the severity levels for validation errors.
/// </summary>
public enum ValidationSeverity
{
    /// <summary>
    /// An error severity validation failure. The request should not proceed.
    /// </summary>
    Error = 0,

    /// <summary>
    /// A warning severity validation failure. The request may proceed but may not behave as expected.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// An informational validation message. The request can safely proceed.
    /// </summary>
    Info = 2
}
