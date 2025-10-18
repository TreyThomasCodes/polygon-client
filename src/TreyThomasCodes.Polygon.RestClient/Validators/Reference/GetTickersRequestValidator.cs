// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using System.Text.RegularExpressions;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Reference;

/// <summary>
/// Validator for <see cref="GetTickersRequest"/>.
/// </summary>
public partial class GetTickersRequestValidator : AbstractValidator<GetTickersRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTickersRequestValidator"/> class.
    /// </summary>
    public GetTickersRequestValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Ticker), () =>
        {
            RuleFor(x => x.Ticker)
                .MaximumLength(10)
                .WithMessage("Ticker must not exceed 10 characters.");
        });

        When(x => !string.IsNullOrEmpty(x.Date), () =>
        {
            RuleFor(x => x.Date)
                .Matches(DateRegex())
                .WithMessage("Date must be in YYYY-MM-DD format.");
        });

        When(x => x.Limit.HasValue, () =>
        {
            RuleFor(x => x.Limit!.Value)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000)
                .WithMessage("Limit must be between 1 and 1000.");
        });
    }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$")]
    private static partial Regex DateRegex();
}
