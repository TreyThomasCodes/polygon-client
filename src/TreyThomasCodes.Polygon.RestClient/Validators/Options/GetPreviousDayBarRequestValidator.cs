// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Options;

/// <summary>
/// Validator for <see cref="GetPreviousDayBarRequest"/>.
/// </summary>
public class GetPreviousDayBarRequestValidator : AbstractValidator<GetPreviousDayBarRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPreviousDayBarRequestValidator"/> class.
    /// </summary>
    public GetPreviousDayBarRequestValidator()
    {
        RuleFor(x => x.OptionsTicker)
            .NotEmpty()
            .WithMessage("Options ticker must not be empty.")
            .Must(BeValidOptionsTicker)
            .WithMessage("Options ticker must be in valid OCC format (e.g., 'O:SPY251219C00650000').");
    }

    private static bool BeValidOptionsTicker(string ticker)
    {
        return OptionsTicker.TryParse(ticker, out _);
    }
}
