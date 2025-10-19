// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent;

/// <summary>
/// Fluent API extension methods for the IReferenceDataService.
/// These extensions provide a more expressive and chainable API for building reference data queries.
/// To use these extensions, add "using TreyThomasCodes.Polygon.RestClient.Fluent;" to your file.
/// </summary>
public static class ReferenceDataServiceFluentExtensions
{
    /// <summary>
    /// Initiates a fluent query builder for retrieving tickers.
    /// </summary>
    /// <param name="service">The reference data service instance.</param>
    /// <returns>A fluent query builder for configuring and executing the tickers request.</returns>
    public static TickersQueryBuilder Tickers(this IReferenceDataService service)
    {
        return new TickersQueryBuilder(service);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving ticker details.
    /// </summary>
    /// <param name="service">The reference data service instance.</param>
    /// <param name="ticker">Optional ticker symbol to initialize the query with (e.g., "AAPL", "MSFT").</param>
    /// <returns>A fluent query builder for configuring and executing the ticker details request.</returns>
    public static TickerDetailsQueryBuilder TickerDetails(this IReferenceDataService service, string? ticker = null)
    {
        return new TickerDetailsQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving market status information.
    /// </summary>
    /// <param name="service">The reference data service instance.</param>
    /// <returns>A fluent query builder for configuring and executing the market status request.</returns>
    public static MarketStatusQueryBuilder MarketStatus(this IReferenceDataService service)
    {
        return new MarketStatusQueryBuilder(service);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving ticker types.
    /// </summary>
    /// <param name="service">The reference data service instance.</param>
    /// <returns>A fluent query builder for configuring and executing the ticker types request.</returns>
    public static TickerTypesQueryBuilder TickerTypes(this IReferenceDataService service)
    {
        return new TickerTypesQueryBuilder(service);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving condition codes.
    /// </summary>
    /// <param name="service">The reference data service instance.</param>
    /// <returns>A fluent query builder for configuring and executing the condition codes request.</returns>
    public static ConditionCodesQueryBuilder ConditionCodes(this IReferenceDataService service)
    {
        return new ConditionCodesQueryBuilder(service);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving exchanges.
    /// </summary>
    /// <param name="service">The reference data service instance.</param>
    /// <returns>A fluent query builder for configuring and executing the exchanges request.</returns>
    public static ExchangesQueryBuilder Exchanges(this IReferenceDataService service)
    {
        return new ExchangesQueryBuilder(service);
    }
}
