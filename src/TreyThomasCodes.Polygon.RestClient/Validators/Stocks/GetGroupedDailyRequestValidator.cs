// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using System.Text.RegularExpressions;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Stocks;

/// <summary>
/// Validator for <see cref="GetGroupedDailyRequest"/>.
/// </summary>
public partial class GetGroupedDailyRequestValidator : AbstractValidator<GetGroupedDailyRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetGroupedDailyRequestValidator"/> class.
    /// </summary>
    public GetGroupedDailyRequestValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date must not be empty.")
            .Matches(DateRegex())
            .WithMessage("Date must be in YYYY-MM-DD format.");
    }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$")]
    private static partial Regex DateRegex();
}
