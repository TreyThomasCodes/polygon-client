// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using System.Text.RegularExpressions;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Options;

/// <summary>
/// Validator for <see cref="GetBarsRequest"/>.
/// </summary>
public partial class GetBarsRequestValidator : AbstractValidator<GetBarsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetBarsRequestValidator"/> class.
    /// </summary>
    public GetBarsRequestValidator()
    {
        RuleFor(x => x.OptionsTicker)
            .NotEmpty()
            .WithMessage("Options ticker must not be empty.")
            .Must(BeValidOptionsTicker)
            .WithMessage("Options ticker must be in valid OCC format (e.g., 'O:SPY251219C00650000').");

        RuleFor(x => x.Multiplier)
            .GreaterThan(0)
            .WithMessage("Multiplier must be greater than 0.");

        RuleFor(x => x.From)
            .NotEmpty()
            .WithMessage("From date must not be empty.")
            .Matches(DateRegex())
            .WithMessage("From date must be in YYYY-MM-DD format.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("To date must not be empty.")
            .Matches(DateRegex())
            .WithMessage("To date must be in YYYY-MM-DD format.");

        When(x => x.Limit.HasValue, () =>
        {
            RuleFor(x => x.Limit!.Value)
                .GreaterThan(0)
                .WithMessage("Limit must be greater than 0.");
        });
    }

    private static bool BeValidOptionsTicker(string ticker)
    {
        return OptionsTicker.TryParse(ticker, out _);
    }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$")]
    private static partial Regex DateRegex();
}
