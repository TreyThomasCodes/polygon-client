// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.RestClient.Services;

/// <summary>
/// Main client implementation for accessing Polygon.io financial market data APIs.
/// Serves as the primary entry point for all Polygon.io API operations, providing
/// centralized access to stocks and reference data services.
/// </summary>
public class PolygonClient : IPolygonClient
{
    /// <summary>
    /// Gets the stocks service for accessing real-time and historical stock market data.
    /// Provides access to trades, quotes, and snapshots for individual stock tickers.
    /// </summary>
    public IStocksService Stocks { get; }

    /// <summary>
    /// Gets the reference data service for accessing market status and other reference information.
    /// Provides access to market trading status, exchange information, and other metadata.
    /// </summary>
    public IReferenceDataService ReferenceData { get; }

    /// <summary>
    /// Gets the options service for accessing options contract data and market information.
    /// Provides access to options trades, quotes, snapshots, and aggregates for options contracts.
    /// </summary>
    public IOptionsService Options { get; }

    /// <summary>
    /// Initializes a new instance of the PolygonClient with the required services.
    /// </summary>
    /// <param name="stocks">The stocks service for market data operations.</param>
    /// <param name="referenceData">The reference data service for market status and metadata operations.</param>
    /// <param name="options">The options service for options contract data and market operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the required services is null.</exception>
    public PolygonClient(
        IStocksService stocks,
        IReferenceDataService referenceData,
        IOptionsService options)
    {
        Stocks = stocks ?? throw new ArgumentNullException(nameof(stocks));
        ReferenceData = referenceData ?? throw new ArgumentNullException(nameof(referenceData));
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }
}