// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Stocks;

/// <summary>
/// Validator for <see cref="GetMarketSnapshotRequest"/>.
/// </summary>
public class GetMarketSnapshotRequestValidator : AbstractValidator<GetMarketSnapshotRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetMarketSnapshotRequestValidator"/> class.
    /// </summary>
    public GetMarketSnapshotRequestValidator()
    {
        // No validation rules needed - all properties are optional
    }
}
