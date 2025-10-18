// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Reference;

/// <summary>
/// Validator for <see cref="GetConditionCodesRequest"/>.
/// </summary>
public class GetConditionCodesRequestValidator : AbstractValidator<GetConditionCodesRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetConditionCodesRequestValidator"/> class.
    /// </summary>
    public GetConditionCodesRequestValidator()
    {
        When(x => x.Limit.HasValue, () =>
        {
            RuleFor(x => x.Limit!.Value)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000)
                .WithMessage("Limit must be between 1 and 1000.");
        });
    }
}
