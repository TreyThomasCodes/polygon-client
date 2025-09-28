// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TTC.Polygon.RestClient.Services;

/// <summary>
/// Main client interface for accessing Polygon.io financial market data APIs.
/// Provides access to different market data services including stocks, aggregates, tickers, and reference data.
/// </summary>
public interface IPolygonClient
{
    /// <summary>
    /// Gets the stocks service for accessing real-time and historical stock market data.
    /// Provides access to trades, quotes, and snapshots for individual stock tickers.
    /// </summary>
    IStocksService Stocks { get; }

    /// <summary>
    /// Gets the reference data service for accessing market status and other reference information.
    /// Provides access to market trading status, exchange information, and other metadata.
    /// </summary>
    IReferenceDataService ReferenceData { get; }
}