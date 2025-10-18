// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Reference;

/// <summary>
/// Validator for <see cref="GetExchangesRequest"/>.
/// </summary>
public class GetExchangesRequestValidator : AbstractValidator<GetExchangesRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetExchangesRequestValidator"/> class.
    /// </summary>
    public GetExchangesRequestValidator()
    {
        // No validation rules needed - all properties are optional enums
    }
}
