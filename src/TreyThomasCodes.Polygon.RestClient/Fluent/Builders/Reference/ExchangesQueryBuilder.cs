// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Reference;

/// <summary>
/// Fluent query builder for constructing and executing exchanges list requests.
/// This builder provides a progressive, chainable API for retrieving exchange and market center information.
/// </summary>
public class ExchangesQueryBuilder
{
    private readonly IReferenceDataService _service;
    private AssetClass? _assetClass;
    private Locale? _locale;

    /// <summary>
    /// Initializes a new instance of the ExchangesQueryBuilder.
    /// </summary>
    /// <param name="service">The reference data service to execute the request against.</param>
    public ExchangesQueryBuilder(IReferenceDataService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Filters exchanges by asset class.
    /// </summary>
    /// <param name="assetClass">The asset class to filter by.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ExchangesQueryBuilder ForAssetClass(AssetClass assetClass)
    {
        _assetClass = assetClass;
        return this;
    }

    /// <summary>
    /// Filters exchanges by locale.
    /// </summary>
    /// <param name="locale">The locale to filter by.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ExchangesQueryBuilder InLocale(Locale locale)
    {
        _locale = locale;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the exchanges response.</returns>
    public Task<PolygonResponse<List<Exchange>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = new GetExchangesRequest
        {
            AssetClass = _assetClass,
            Locale = _locale
        };

        return _service.GetExchangesAsync(request, cancellationToken);
    }
}
