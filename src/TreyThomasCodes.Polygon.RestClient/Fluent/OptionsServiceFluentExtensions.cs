// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent;

/// <summary>
/// Fluent API extension methods for the IOptionsService.
/// These extensions provide a more expressive and chainable API for building options data queries.
/// To use these extensions, add "using TreyThomasCodes.Polygon.RestClient.Fluent;" to your file.
/// </summary>
public static class OptionsServiceFluentExtensions
{
    /// <summary>
    /// Initiates a fluent query builder for retrieving options contract details.
    /// </summary>
    /// <param name="service">The options service instance.</param>
    /// <param name="ticker">Optional options ticker to initialize the query with (e.g., "O:TSLA260320C00700000").</param>
    /// <returns>A fluent query builder for configuring and executing the contract details request.</returns>
    public static ContractDetailsQueryBuilder ContractDetails(this IOptionsService service, string? ticker = null)
    {
        return new ContractDetailsQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving an options contract snapshot.
    /// </summary>
    /// <param name="service">The options service instance.</param>
    /// <param name="ticker">Optional options ticker to initialize the query with.</param>
    /// <returns>A fluent query builder for configuring and executing the snapshot request.</returns>
    public static OptionSnapshotQueryBuilder OptionSnapshot(this IOptionsService service, string? ticker = null)
    {
        return new OptionSnapshotQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving an options chain snapshot.
    /// </summary>
    /// <param name="service">The options service instance.</param>
    /// <param name="underlying">Optional underlying asset ticker to initialize the query with (e.g., "SPY", "AAPL").</param>
    /// <returns>A fluent query builder for configuring and executing the chain snapshot request.</returns>
    public static ChainSnapshotQueryBuilder ChainSnapshot(this IOptionsService service, string? underlying = null)
    {
        return new ChainSnapshotQueryBuilder(service, underlying);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving aggregate bars (OHLC) for an options contract.
    /// </summary>
    /// <param name="service">The options service instance.</param>
    /// <param name="ticker">Optional options ticker to initialize the query with.</param>
    /// <returns>A fluent query builder for configuring and executing the bars request.</returns>
    public static OptionBarsQueryBuilder OptionBars(this IOptionsService service, string? ticker = null)
    {
        return new OptionBarsQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving trade data for an options contract.
    /// </summary>
    /// <param name="service">The options service instance.</param>
    /// <param name="ticker">Optional options ticker to initialize the query with.</param>
    /// <returns>A fluent query builder for configuring and executing the trades request.</returns>
    public static OptionTradesQueryBuilder OptionTrades(this IOptionsService service, string? ticker = null)
    {
        return new OptionTradesQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving quote data for an options contract.
    /// </summary>
    /// <param name="service">The options service instance.</param>
    /// <param name="ticker">Optional options ticker to initialize the query with.</param>
    /// <returns>A fluent query builder for configuring and executing the quotes request.</returns>
    public static OptionQuotesQueryBuilder OptionQuotes(this IOptionsService service, string? ticker = null)
    {
        return new OptionQuotesQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving the most recent trade for an options contract.
    /// </summary>
    /// <param name="service">The options service instance.</param>
    /// <param name="ticker">Optional options ticker to initialize the query with.</param>
    /// <returns>A fluent query builder for configuring and executing the last trade request.</returns>
    public static OptionLastTradeQueryBuilder OptionLastTrade(this IOptionsService service, string? ticker = null)
    {
        return new OptionLastTradeQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving daily open/close data for an options contract.
    /// </summary>
    /// <param name="service">The options service instance.</param>
    /// <param name="ticker">Optional options ticker to initialize the query with.</param>
    /// <returns>A fluent query builder for configuring and executing the daily open/close request.</returns>
    public static OptionDailyOpenCloseQueryBuilder OptionDailyOpenClose(this IOptionsService service, string? ticker = null)
    {
        return new OptionDailyOpenCloseQueryBuilder(service, ticker);
    }

    /// <summary>
    /// Initiates a fluent query builder for retrieving the previous day's bar for an options contract.
    /// </summary>
    /// <param name="service">The options service instance.</param>
    /// <param name="ticker">Optional options ticker to initialize the query with.</param>
    /// <returns>A fluent query builder for configuring and executing the previous day bar request.</returns>
    public static OptionPreviousDayBarQueryBuilder OptionPreviousDayBar(this IOptionsService service, string? ticker = null)
    {
        return new OptionPreviousDayBarQueryBuilder(service, ticker);
    }
}
