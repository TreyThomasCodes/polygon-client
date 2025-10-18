// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Options;

/// <summary>
/// Validator for <see cref="GetContractDetailsRequest"/>.
/// </summary>
public class GetContractDetailsRequestValidator : AbstractValidator<GetContractDetailsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetContractDetailsRequestValidator"/> class.
    /// </summary>
    public GetContractDetailsRequestValidator()
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
