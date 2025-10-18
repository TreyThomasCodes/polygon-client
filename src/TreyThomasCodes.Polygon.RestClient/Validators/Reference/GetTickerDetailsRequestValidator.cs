// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using System.Text.RegularExpressions;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Reference;

/// <summary>
/// Validator for <see cref="GetTickerDetailsRequest"/>.
/// </summary>
public partial class GetTickerDetailsRequestValidator : AbstractValidator<GetTickerDetailsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTickerDetailsRequestValidator"/> class.
    /// </summary>
    public GetTickerDetailsRequestValidator()
    {
        RuleFor(x => x.Ticker)
            .NotEmpty()
            .WithMessage("Ticker must not be empty.")
            .MaximumLength(10)
            .WithMessage("Ticker must not exceed 10 characters.");

        When(x => !string.IsNullOrEmpty(x.Date), () =>
        {
            RuleFor(x => x.Date)
                .Matches(DateRegex())
                .WithMessage("Date must be in YYYY-MM-DD format.");
        });
    }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$")]
    private static partial Regex DateRegex();
}
