// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Options;

/// <summary>
/// Fluent query builder for constructing and executing options last trade requests.
/// This builder provides a progressive, chainable API for retrieving the most recent trade for an options contract.
/// </summary>
public class OptionLastTradeQueryBuilder
{
    private readonly IOptionsService _service;
    private string? _optionsTicker;

    /// <summary>
    /// Initializes a new instance of the OptionLastTradeQueryBuilder.
    /// </summary>
    /// <param name="service">The options service to execute the request against.</param>
    /// <param name="ticker">Optional initial options ticker symbol in OCC format.</param>
    public OptionLastTradeQueryBuilder(IOptionsService service, string? ticker = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _optionsTicker = ticker;
    }

    /// <summary>
    /// Specifies the options ticker symbol for the query.
    /// </summary>
    /// <param name="ticker">The options ticker symbol in OCC format (e.g., "O:TSLA260320C00700000").</param>
    /// <returns>The builder instance for method chaining.</returns>
    public OptionLastTradeQueryBuilder ForTicker(string ticker)
    {
        _optionsTicker = ticker;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the last trade response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters are not set.</exception>
    public Task<PolygonResponse<OptionTrade>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_optionsTicker))
            throw new InvalidOperationException("Options ticker is required. Use ForTicker() to specify the options ticker symbol.");

        var request = new GetLastTradeRequest
        {
            OptionsTicker = _optionsTicker
        };

        return _service.GetLastTradeAsync(request, cancellationToken);
    }
}
