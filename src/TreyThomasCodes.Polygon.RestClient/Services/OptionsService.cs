// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.RestClient.Api;

namespace TreyThomasCodes.Polygon.RestClient.Services;

/// <summary>
/// Implementation of the options service for accessing Polygon.io options market data.
/// Provides methods to retrieve options contract information, trades, quotes, snapshots, and OHLC aggregates.
/// This service acts as a wrapper around the Polygon.io Options API, providing a convenient interface for accessing options data.
/// </summary>
public class OptionsService : IOptionsService
{
    private readonly IPolygonOptionsApi _api;

    /// <summary>
    /// Initializes a new instance of the OptionsService with the specified API client.
    /// </summary>
    /// <param name="api">The Polygon.io options API client used for making HTTP requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when the api parameter is null.</exception>
    public OptionsService(IPolygonOptionsApi api)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
    }

    // Options service method implementations will be added here as API endpoints are defined.
    // Each method will delegate to the corresponding API method and may add additional
    // business logic, validation, or data transformation as needed.
}
