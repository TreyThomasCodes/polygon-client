// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Stocks;

/// <summary>
/// Validator for <see cref="GetQuotesRequest"/>.
/// </summary>
public class GetQuotesRequestValidator : AbstractValidator<GetQuotesRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetQuotesRequestValidator"/> class.
    /// </summary>
    public GetQuotesRequestValidator()
    {
        RuleFor(x => x.Ticker)
            .NotEmpty()
            .WithMessage("Ticker must not be empty.")
            .MaximumLength(10)
            .WithMessage("Ticker must not exceed 10 characters.");

        When(x => x.Limit.HasValue, () =>
        {
            RuleFor(x => x.Limit!.Value)
                .GreaterThan(0)
                .WithMessage("Limit must be greater than 0.");
        });
    }
}
