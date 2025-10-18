// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Reference;

/// <summary>
/// Validator for <see cref="GetMarketStatusRequest"/>.
/// </summary>
public class GetMarketStatusRequestValidator : AbstractValidator<GetMarketStatusRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetMarketStatusRequestValidator"/> class.
    /// </summary>
    public GetMarketStatusRequestValidator()
    {
        // No validation rules needed - this endpoint requires no parameters
    }
}
