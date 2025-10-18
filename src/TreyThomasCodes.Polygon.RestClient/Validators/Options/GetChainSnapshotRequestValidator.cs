// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using FluentValidation;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using System.Text.RegularExpressions;

namespace TreyThomasCodes.Polygon.RestClient.Validators.Options;

/// <summary>
/// Validator for <see cref="GetChainSnapshotRequest"/>.
/// </summary>
public partial class GetChainSnapshotRequestValidator : AbstractValidator<GetChainSnapshotRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetChainSnapshotRequestValidator"/> class.
    /// </summary>
    public GetChainSnapshotRequestValidator()
    {
        RuleFor(x => x.UnderlyingAsset)
            .NotEmpty()
            .WithMessage("Underlying asset ticker must not be empty.")
            .MaximumLength(10)
            .WithMessage("Underlying asset ticker must not exceed 10 characters.");

        When(x => x.StrikePrice.HasValue, () =>
        {
            RuleFor(x => x.StrikePrice!.Value)
                .GreaterThan(0)
                .WithMessage("Strike price must be greater than 0.");
        });

        When(x => !string.IsNullOrEmpty(x.ContractType), () =>
        {
            RuleFor(x => x.ContractType)
                .Must(type => type == "call" || type == "put")
                .WithMessage("Contract type must be 'call' or 'put'.");
        });

        When(x => !string.IsNullOrEmpty(x.ExpirationDateGte), () =>
        {
            RuleFor(x => x.ExpirationDateGte)
                .Matches(DateRegex())
                .WithMessage("Expiration date (gte) must be in YYYY-MM-DD format.");
        });

        When(x => !string.IsNullOrEmpty(x.ExpirationDateLte), () =>
        {
            RuleFor(x => x.ExpirationDateLte)
                .Matches(DateRegex())
                .WithMessage("Expiration date (lte) must be in YYYY-MM-DD format.");
        });

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

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$")]
    private static partial Regex DateRegex();
}
