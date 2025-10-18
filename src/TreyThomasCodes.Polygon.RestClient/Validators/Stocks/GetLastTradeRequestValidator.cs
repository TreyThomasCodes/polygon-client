// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Stocks;

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
        RuleFor(x => x.Ticker)
            .NotEmpty()
            .WithMessage("Ticker must not be empty.")
            .MaximumLength(10)
            .WithMessage("Ticker must not exceed 10 characters.");
    }
}
