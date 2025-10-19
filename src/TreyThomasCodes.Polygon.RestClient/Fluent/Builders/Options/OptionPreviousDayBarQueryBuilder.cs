// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Options;

/// <summary>
/// Fluent query builder for constructing and executing options previous day bar requests.
/// This builder provides a progressive, chainable API for retrieving the previous trading day's OHLC data.
/// </summary>
public class OptionPreviousDayBarQueryBuilder
{
    private readonly IOptionsService _service;
    private string? _optionsTicker;
    private bool? _adjusted;

    /// <summary>
    /// Initializes a new instance of the OptionPreviousDayBarQueryBuilder.
    /// </summary>
    /// <param name="service">The options service to execute the request against.</param>
    /// <param name="ticker">Optional initial options ticker symbol.</param>
    public OptionPreviousDayBarQueryBuilder(IOptionsService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _optionsTicker = ticker;
    }

    /// <summary>
    /// Specifies the options ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The options ticker symbol in OCC format (e.g., "O:SPY251219C00650000").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionPreviousDayBarQueryBuilder ForTicker(string ticker)
    {
        _optionsTicker = ticker;
        return this;
    }

    /// <summary>
    /// Specifies whether the results should be adjusted for underlying stock splits.
    /// </summary>
    /// <param name="adjusted">True to adjust for splits, false otherwise.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionPreviousDayBarQueryBuilder Adjusted(bool adjusted = true)
    {
        _adjusted = adjusted;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the previous day bar response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<List<OptionBar>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_optionsTicker))
            throw new InvalidOperationException("Options ticker is required. Use ForTicker() to specify the options ticker symbol.");

        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = _optionsTicker,
            Adjusted = _adjusted
        };

        return _service.GetPreviousDayBarAsync(request, cancellationToken);
    }
}
