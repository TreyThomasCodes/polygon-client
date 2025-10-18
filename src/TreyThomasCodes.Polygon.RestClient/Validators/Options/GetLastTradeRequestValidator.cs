// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Options;

/// <summary>
/// Validator for <see cref="GetLastTradeRequest"/>.
/// </summary>
public class GetLastTradeRequestValidator : AbstractValidator<GetLastTradeRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetLastTradeRequestValidator"/> class.
    /// </summary>
    public GetLastTradeRequestValidator()
    {
        RuleFor(x => x.OptionsTicker)
            .NotEmpty()
            .WithMessage("Options ticker must not be empty.")
            .Must(BeValidOptionsTicker)
            .WithMessage("Options ticker must be in valid OCC format (e.g., 'O:TSLA260320C00700000').");
    }

    private static bool BeValidOptionsTicker(string ticker)
    {
        return OptionsTicker.TryParse(ticker, out _);
    }
}
