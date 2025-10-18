// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using System.Text.RegularExpressions;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Options;

/// <summary>
/// Validator for <see cref="GetDailyOpenCloseRequest"/>.
/// </summary>
public partial class GetDailyOpenCloseRequestValidator : AbstractValidator<GetDailyOpenCloseRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetDailyOpenCloseRequestValidator"/> class.
    /// </summary>
    public GetDailyOpenCloseRequestValidator()
    {
        RuleFor(x => x.OptionsTicker)
            .NotEmpty()
            .WithMessage("Options ticker must not be empty.")
            .Must(BeValidOptionsTicker)
            .WithMessage("Options ticker must be in valid OCC format (e.g., 'O:SPY251219C00650000').");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date must not be empty.")
            .Matches(DateRegex())
            .WithMessage("Date must be in YYYY-MM-DD format (e.g., '2023-01-09').");
    }

    private static bool BeValidOptionsTicker(string ticker)
    {
        return OptionsTicker.TryParse(ticker, out _);
    }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$")]
    private static partial Regex DateRegex();
}
