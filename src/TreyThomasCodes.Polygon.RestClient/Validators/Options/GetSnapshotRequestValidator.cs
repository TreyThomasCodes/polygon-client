// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Options;

/// <summary>
/// Validator for <see cref="GetSnapshotRequest"/>.
/// </summary>
public class GetSnapshotRequestValidator : AbstractValidator<GetSnapshotRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSnapshotRequestValidator"/> class.
    /// </summary>
    public GetSnapshotRequestValidator()
    {
        RuleFor(x => x.UnderlyingAsset)
            .NotEmpty()
            .WithMessage("Underlying asset ticker must not be empty.")
            .MaximumLength(10)
            .WithMessage("Underlying asset ticker must not exceed 10 characters.");

        RuleFor(x => x.OptionContract)
            .NotEmpty()
            .WithMessage("Option contract must not be empty.")
            .MinimumLength(15)
            .WithMessage("Option contract must be at least 15 characters (OCC format without 'O:' prefix).");
    }
}
