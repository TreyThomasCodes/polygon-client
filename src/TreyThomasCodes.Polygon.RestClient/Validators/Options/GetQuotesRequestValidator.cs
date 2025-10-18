// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Options;

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
        RuleFor(x => x.OptionsTicker)
            .NotEmpty()
            .WithMessage("Options ticker must not be empty.")
            .Must(BeValidOptionsTicker)
            .WithMessage("Options ticker must be in valid OCC format (e.g., 'O:SPY241220P00720000').");

        When(x => x.Limit.HasValue, () =>
        {
            RuleFor(x => x.Limit!.Value)
                .GreaterThan(0)
                .WithMessage("Limit must be greater than 0.");
        });

        When(x => !string.IsNullOrEmpty(x.Order), () =>
        {
            RuleFor(x => x.Order)
                .Must(order => order == "asc" || order == "desc")
                .WithMessage("Order must be 'asc' or 'desc'.");
        });
    }

    private static bool BeValidOptionsTicker(string ticker)
    {
        return OptionsTicker.TryParse(ticker, out _);
    }
}
