// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Reference;

/// <summary>
/// Fluent query builder for constructing and executing market status requests.
/// This builder provides a progressive, chainable API for retrieving current trading status information.
/// </summary>
public class MarketStatusQueryBuilder
{
    private readonly IReferenceDataService _service;

    /// <summary>
    /// Initializes a new instance of the MarketStatusQueryBuilder.
    /// </summary>
    /// <param name="service">The reference data service to execute the request against.</param>
    public MarketStatusQueryBuilder(IReferenceDataService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the market status response.</returns>
    public Task<MarketStatus> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = new GetMarketStatusRequest();
        return _service.GetMarketStatusAsync(request, cancellationToken);
    }
}
