// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Options;

/// <summary>
/// Fluent query builder for constructing and executing options contract snapshot requests.
/// This builder provides a progressive, chainable API for retrieving current market data for a specific options contract.
/// </summary>
public class OptionSnapshotQueryBuilder
{
    private readonly IOptionsService _service;
    private string? _underlyingAsset;
    private string? _optionContract;

    /// <summary>
    /// Initializes a new instance of the OptionSnapshotQueryBuilder.
    /// </summary>
    /// <param name="service">The options service to execute the request against.</param>
    /// <param name="ticker">Optional initial options ticker symbol in OCC format (with or without "O:" prefix).</param>
    public OptionSnapshotQueryBuilder(IOptionsService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));

        if (!string.IsNullOrWhiteSpace(ticker))
        {
            ParseTicker(ticker);
        }
    }

    /// <summary>
    /// Specifies the options ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The options ticker symbol in OCC format (with or without "O:" prefix, e.g., "O:SPY251219C00650000" or "SPY251219C00650000").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionSnapshotQueryBuilder ForTicker(string ticker)
    {
        ParseTicker(ticker);
        return this;
    }

    /// <summary>
    /// Specifies the underlying asset ticker symbol for the query.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY", "AAPL").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionSnapshotQueryBuilder ForUnderlying(string underlying)
    {
        _underlyingAsset = underlying;
        return this;
    }

    /// <summary>
    /// Specifies the options contract identifier for the query.
    /// </summary>
    /// <param name="contract">The options contract identifier in OCC format without the "O:" prefix (e.g., "SPY251219C00650000").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionSnapshotQueryBuilder WithContract(string contract)
    {
        _optionContract = contract;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the option snapshot response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<OptionSnapshot>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_underlyingAsset))
            throw new InvalidOperationException("Underlying asset is required. Use ForUnderlying() or ForTicker() to specify the underlying asset.");

        if (string.IsNullOrWhiteSpace(_optionContract))
            throw new InvalidOperationException("Option contract is required. Use WithContract() or ForTicker() to specify the option contract.");

        var request = new GetSnapshotRequest
        {
            UnderlyingAsset = _underlyingAsset,
            OptionContract = _optionContract
        };

        return _service.GetSnapshotAsync(request, cancellationToken);
    }

    private void ParseTicker(string ticker)
    {
        // Remove "O:" prefix if present
        var cleanTicker = ticker.StartsWith("O:", StringComparison.OrdinalIgnoreCase)
            ? ticker.Substring(2)
            : ticker;

        // Extract underlying (everything before the date pattern)
        // OCC format: UNDERLYING + YYMMDD + C/P + Strike (8 digits)
        // Example: SPY251219C00650000
        // Minimum length check: at least 15 characters (1 char underlying + YYMMDD + C/P + 00000000)
        if (cleanTicker.Length < 15)
        {
            _optionContract = cleanTicker;
            return;
        }

        // Find where the date starts by looking for the pattern YYMMDD
        // The underlying can be 1-6 characters
        for (int i = 1; i <= 6 && i <= cleanTicker.Length - 14; i++)
        {
            var potential = cleanTicker.Substring(0, i);
            var remaining = cleanTicker.Substring(i);

            // Check if remaining matches OCC pattern: YYMMDD + C/P + 8 digits
            if (remaining.Length >= 15 &&
                char.IsDigit(remaining[0]) && char.IsDigit(remaining[1]) && // YY
                char.IsDigit(remaining[2]) && char.IsDigit(remaining[3]) && // MM
                char.IsDigit(remaining[4]) && char.IsDigit(remaining[5]) && // DD
                (remaining[6] == 'C' || remaining[6] == 'P' || remaining[6] == 'c' || remaining[6] == 'p'))
            {
                _underlyingAsset = potential;
                _optionContract = remaining;
                return;
            }
        }

        // Fallback: just use the clean ticker as the contract
        _optionContract = cleanTicker;
    }
}
