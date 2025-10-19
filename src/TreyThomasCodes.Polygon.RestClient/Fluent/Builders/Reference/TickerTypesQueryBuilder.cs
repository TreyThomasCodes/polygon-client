// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Reference;

/// <summary>
/// Fluent query builder for constructing and executing ticker types requests.
/// This builder provides a progressive, chainable API for retrieving all supported ticker types.
/// </summary>
public class TickerTypesQueryBuilder
{
    private readonly IReferenceDataService _service;

    /// <summary>
    /// Initializes a new instance of the TickerTypesQueryBuilder.
    /// </summary>
    /// <param name="service">The reference data service to execute the request against.</param>
    public TickerTypesQueryBuilder(IReferenceDataService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the ticker types response.</returns>
    public Task<TickerTypesResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = new GetTickerTypesRequest();
        return _service.GetTickerTypesAsync(request, cancellationToken);
    }
}
