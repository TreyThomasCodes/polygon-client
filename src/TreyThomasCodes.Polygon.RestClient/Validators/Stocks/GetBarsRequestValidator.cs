// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using System.Text.RegularExpressions;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Stocks;

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
        RuleFor(x => x.Ticker)
            .NotEmpty()
            .WithMessage("Ticker must not be empty.")
            .MaximumLength(10)
            .WithMessage("Ticker must not exceed 10 characters.");

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
                .LessThanOrEqualTo(50000)
                .WithMessage("Limit must be between 1 and 50000.");
        });
    }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$")]
    private static partial Regex DateRegex();
}
