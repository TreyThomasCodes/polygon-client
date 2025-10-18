// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IOptionsService"/> to simplify common options operations.
/// These helper methods make it easier to work with options contracts without manually constructing ticker strings.
/// </summary>
public static class OptionsServiceExtensions
{
    /// <summary>
    /// Retrieves detailed information about a specific options contract using its component parts instead of a pre-formatted ticker.
    /// This is a convenience method that constructs the OCC format ticker string from the provided components.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "UBER", "SPY", "AAPL").</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract (e.g., 50 for $50).</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService is null.</exception>
    /// <example>
    /// <code>
    /// // Get contract details for UBER January 21, 2022 $50 Call
    /// var contract = await client.Options.GetContractByComponentsAsync(
    ///     "UBER",
    ///     new DateTime(2022, 1, 21),
    ///     OptionType.Call,
    ///     50m
    /// );
    /// </code>
    /// </example>
    public static Task<PolygonResponse<OptionsContract>> GetContractByComponentsAsync(
        this IOptionsService optionsService,
        string underlying,
        DateTime expirationDate,
        OptionType type,
        decimal strike,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);

        string ticker = OptionsTicker.Create(underlying, expirationDate, type, strike);
        var request = new GetContractDetailsRequest { OptionsTicker = ticker };
        return optionsService.GetContractDetailsAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves a snapshot of current market data for a specific options contract using its component parts.
    /// This is a convenience method that constructs the ticker from the provided components.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY", "AAPL").</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract snapshot.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService is null.</exception>
    /// <example>
    /// <code>
    /// // Get snapshot for SPY December 19, 2025 $650 Call
    /// var snapshot = await client.Options.GetSnapshotByComponentsAsync(
    ///     "SPY",
    ///     new DateTime(2025, 12, 19),
    ///     OptionType.Call,
    ///     650m
    /// );
    /// </code>
    /// </example>
    public static Task<PolygonResponse<OptionSnapshot>> GetSnapshotByComponentsAsync(
        this IOptionsService optionsService,
        string underlying,
        DateTime expirationDate,
        OptionType type,
        decimal strike,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);

        var ticker = new OptionsTicker(underlying, expirationDate, type, strike);
        string optionContract = ticker.ToString().Substring(2); // Remove "O:" prefix
        var request = new GetSnapshotRequest
        {
            UnderlyingAsset = underlying,
            OptionContract = optionContract
        };
        return optionsService.GetSnapshotAsync(request, cancellationToken);
    }

    /// <summary>
    /// Discovers available strike prices for options on a given underlying asset.
    /// This method queries the options chain and returns a sorted list of unique strike prices.
    /// Optionally filters by expiration date range and option type.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY", "AAPL").</param>
    /// <param name="type">Optional filter for option type (Call or Put). If null, returns strikes for both.</param>
    /// <param name="expirationDateGte">Optional filter for minimum expiration date in YYYY-MM-DD format.</param>
    /// <param name="expirationDateLte">Optional filter for maximum expiration date in YYYY-MM-DD format.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a sorted list of available strike prices.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService is null.</exception>
    /// <example>
    /// <code>
    /// // Find all available call strikes for SPY expiring in December 2025
    /// var strikes = await client.Options.GetAvailableStrikesAsync(
    ///     "SPY",
    ///     OptionType.Call,
    ///     expirationDateGte: "2025-12-01",
    ///     expirationDateLte: "2025-12-31"
    /// );
    ///
    /// foreach (var strike in strikes)
    /// {
    ///     Console.WriteLine($"Strike: ${strike}");
    /// }
    /// </code>
    /// </example>
    public static async Task<List<decimal>> GetAvailableStrikesAsync(
        this IOptionsService optionsService,
        string underlying,
        OptionType? type = null,
        string? expirationDateGte = null,
        string? expirationDateLte = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);

        string? contractType = type switch
        {
            OptionType.Call => "call",
            OptionType.Put => "put",
            null => null,
            _ => null
        };

        var request = new GetChainSnapshotRequest
        {
            UnderlyingAsset = underlying,
            ContractType = contractType,
            ExpirationDateGte = expirationDateGte,
            ExpirationDateLte = expirationDateLte,
            Limit = 1000, // Get a large batch
            Sort = "strike_price",
            Order = "asc"
        };

        var response = await optionsService.GetChainSnapshotAsync(request, cancellationToken);

        if (response.Results == null || response.Results.Count == 0)
            return new List<decimal>();

        // Extract unique strike prices and sort them
        var strikes = response.Results
            .Select(snapshot => snapshot.Details?.StrikePrice)
            .Where(strike => strike.HasValue)
            .Select(strike => strike!.Value)
            .Distinct()
            .OrderBy(strike => strike)
            .ToList();

        return strikes;
    }

    /// <summary>
    /// Discovers available expiration dates for options on a given underlying asset.
    /// This method queries the options chain and returns a sorted list of unique expiration dates.
    /// Optionally filters by option type and strike price.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY", "AAPL").</param>
    /// <param name="type">Optional filter for option type (Call or Put). If null, returns dates for both.</param>
    /// <param name="strikePrice">Optional filter for a specific strike price.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a sorted list of available expiration dates.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService is null.</exception>
    /// <example>
    /// <code>
    /// // Find all available expiration dates for UBER put options
    /// var expirations = await client.Options.GetExpirationDatesAsync(
    ///     "UBER",
    ///     OptionType.Put
    /// );
    ///
    /// foreach (var date in expirations)
    /// {
    ///     Console.WriteLine($"Expiration: {date:yyyy-MM-dd}");
    /// }
    ///
    /// // Find expiration dates for $50 strike calls on UBER
    /// var expirationsForStrike = await client.Options.GetExpirationDatesAsync(
    ///     "UBER",
    ///     OptionType.Call,
    ///     strikePrice: 50m
    /// );
    /// </code>
    /// </example>
    public static async Task<List<DateTime>> GetExpirationDatesAsync(
        this IOptionsService optionsService,
        string underlying,
        OptionType? type = null,
        decimal? strikePrice = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);

        string? contractType = type switch
        {
            OptionType.Call => "call",
            OptionType.Put => "put",
            null => null,
            _ => null
        };

        var request = new GetChainSnapshotRequest
        {
            UnderlyingAsset = underlying,
            StrikePrice = strikePrice,
            ContractType = contractType,
            Limit = 1000, // Get a large batch
            Sort = "expiration_date",
            Order = "asc"
        };

        var response = await optionsService.GetChainSnapshotAsync(request, cancellationToken);

        if (response.Results == null || response.Results.Count == 0)
            return new List<DateTime>();

        // Extract unique expiration dates and sort them
        var dates = response.Results
            .Select(snapshot => snapshot.Details?.ExpirationDate)
            .Where(date => !string.IsNullOrEmpty(date))
            .Select(date => DateTime.Parse(date!))
            .Distinct()
            .OrderBy(date => date)
            .ToList();

        return dates;
    }

    #region OptionsTicker-based extension methods

    /// <summary>
    /// Retrieves detailed information about a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService or ticker is null.</exception>
    /// <example>
    /// <code>
    /// // Using parsed ticker
    /// var ticker = OptionsTicker.Parse("O:UBER220121C00050000");
    /// var contract = await client.Options.GetContractDetailsAsync(ticker);
    ///
    /// // Using builder pattern
    /// var ticker = new OptionsTickerBuilder()
    ///     .WithUnderlying("UBER")
    ///     .WithExpiration(2022, 1, 21)
    ///     .AsCall()
    ///     .WithStrike(50m)
    ///     .BuildTicker();
    /// var contract = await client.Options.GetContractDetailsAsync(ticker);
    /// </code>
    /// </example>
    public static Task<PolygonResponse<OptionsContract>> GetContractDetailsAsync(
        this IOptionsService optionsService,
        OptionsTicker ticker,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);
        ArgumentNullException.ThrowIfNull(ticker);

        var request = new GetContractDetailsRequest { OptionsTicker = ticker.ToString() };
        return optionsService.GetContractDetailsAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves a snapshot of current market data for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the options contract snapshot.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService or ticker is null.</exception>
    /// <example>
    /// <code>
    /// var ticker = new OptionsTicker("SPY", new DateTime(2025, 12, 19), OptionType.Call, 650m);
    /// var snapshot = await client.Options.GetSnapshotAsync(ticker);
    /// </code>
    /// </example>
    public static Task<PolygonResponse<OptionSnapshot>> GetSnapshotAsync(
        this IOptionsService optionsService,
        OptionsTicker ticker,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);
        ArgumentNullException.ThrowIfNull(ticker);

        string optionContract = ticker.ToString().Substring(2); // Remove "O:" prefix
        var request = new GetSnapshotRequest
        {
            UnderlyingAsset = ticker.Underlying,
            OptionContract = optionContract
        };
        return optionsService.GetSnapshotAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves the most recent trade for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the most recent trade data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService or ticker is null.</exception>
    /// <example>
    /// <code>
    /// var ticker = OptionsTicker.Parse("O:TSLA260320C00700000");
    /// var trade = await client.Options.GetLastTradeAsync(ticker);
    /// </code>
    /// </example>
    public static Task<PolygonResponse<OptionTrade>> GetLastTradeAsync(
        this IOptionsService optionsService,
        OptionsTicker ticker,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);
        ArgumentNullException.ThrowIfNull(ticker);

        var request = new GetLastTradeRequest { OptionsTicker = ticker.ToString() };
        return optionsService.GetLastTradeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves historical quotes (bid/ask) for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns a list of quotes containing bid and ask prices, sizes, exchange information, and timing data.
    /// Supports pagination and time-based filtering.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="timestamp">Query for quotes at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampLt">Query for quotes before this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampLte">Query for quotes at or before this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampGt">Query for quotes after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampGte">Query for quotes at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="order">Sort order for results. Use "asc" for ascending or "desc" for descending by timestamp.</param>
    /// <param name="limit">Limit the number of results returned. Maximum value varies by plan.</param>
    /// <param name="sort">Sort field for results. Defaults to "timestamp".</param>
    /// <param name="cursor">Cursor for pagination. Use the next_url from the previous response to get the next page of results.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option quotes including bid/ask prices, sizes, exchanges, sequence numbers, and timestamps.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService or ticker is null.</exception>
    /// <example>
    /// <code>
    /// var ticker = OptionsTicker.Parse("O:SPY241220P00720000");
    /// var quotes = await client.Options.GetQuotesAsync(
    ///     ticker,
    ///     timestamp: "2024-12-01",
    ///     limit: 100
    /// );
    /// </code>
    /// </example>
    public static Task<PolygonResponse<List<OptionQuote>>> GetQuotesAsync(
        this IOptionsService optionsService,
        OptionsTicker ticker,
        string? timestamp = null,
        string? timestampLt = null,
        string? timestampLte = null,
        string? timestampGt = null,
        string? timestampGte = null,
        string? order = null,
        int? limit = null,
        string? sort = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);
        ArgumentNullException.ThrowIfNull(ticker);

        var request = new GetQuotesRequest
        {
            OptionsTicker = ticker.ToString(),
            Timestamp = timestamp,
            TimestampLt = timestampLt,
            TimestampLte = timestampLte,
            TimestampGt = timestampGt,
            TimestampGte = timestampGte,
            Order = order,
            Limit = limit,
            Sort = sort,
            Cursor = cursor
        };
        return optionsService.GetQuotesAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves historical trade data for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns a list of trades containing price, size, exchange, conditions, and timing data.
    /// Supports pagination and time-based filtering.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="timestamp">Query for trades at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampLt">Query for trades before this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampLte">Query for trades at or before this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampGt">Query for trades after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="timestampGte">Query for trades at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.</param>
    /// <param name="order">Sort order for results. Use "asc" for ascending or "desc" for descending by timestamp.</param>
    /// <param name="limit">Limit the number of results returned. Maximum value varies by plan.</param>
    /// <param name="sort">Sort field for results. Defaults to "timestamp".</param>
    /// <param name="cursor">Cursor for pagination. Use the next_url from the previous response to get the next page of results.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of option trades including price, size, exchange, conditions, sequence numbers, and timestamps.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService or ticker is null.</exception>
    /// <example>
    /// <code>
    /// var ticker = new OptionsTicker("TSLA", new DateTime(2021, 9, 3), OptionType.Call, 700m);
    /// var trades = await client.Options.GetTradesAsync(
    ///     ticker,
    ///     timestamp: "2021-09-03",
    ///     limit: 100
    /// );
    /// </code>
    /// </example>
    public static Task<PolygonResponse<List<OptionTradeV3>>> GetTradesAsync(
        this IOptionsService optionsService,
        OptionsTicker ticker,
        string? timestamp = null,
        string? timestampLt = null,
        string? timestampLte = null,
        string? timestampGt = null,
        string? timestampGte = null,
        string? order = null,
        int? limit = null,
        string? sort = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);
        ArgumentNullException.ThrowIfNull(ticker);

        var request = new GetTradesRequest
        {
            OptionsTicker = ticker.ToString(),
            Timestamp = timestamp,
            TimestampLt = timestampLt,
            TimestampLte = timestampLte,
            TimestampGt = timestampGt,
            TimestampGte = timestampGte,
            Order = order,
            Limit = limit,
            Sort = sort,
            Cursor = cursor
        };
        return optionsService.GetTradesAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves aggregate OHLC (bar/candle) data for an options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns historical pricing data aggregated by the specified time interval, useful for charting and technical analysis.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="multiplier">The number of timespan units to aggregate (e.g., 1 for 1 day, 5 for 5 minutes).</param>
    /// <param name="timespan">The size of the time window for each aggregate (e.g., minute, hour, day).</param>
    /// <param name="from">Start date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="to">End date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="adjusted">Whether to adjust for splits. Defaults to true if not specified.</param>
    /// <param name="sort">Sort order for results (asc for ascending, desc for descending by timestamp).</param>
    /// <param name="limit">Limit the number of aggregate results returned.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of aggregate OHLC bars.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService or ticker is null.</exception>
    /// <example>
    /// <code>
    /// var ticker = new OptionsTickerBuilder()
    ///     .WithUnderlying("SPY")
    ///     .WithExpiration(2025, 12, 19)
    ///     .AsCall()
    ///     .WithStrike(650m)
    ///     .BuildTicker();
    /// var bars = await client.Options.GetBarsAsync(
    ///     ticker,
    ///     multiplier: 1,
    ///     timespan: AggregateInterval.Day,
    ///     from: "2025-11-01",
    ///     to: "2025-11-30"
    /// );
    /// </code>
    /// </example>
    public static Task<PolygonResponse<List<OptionBar>>> GetBarsAsync(
        this IOptionsService optionsService,
        OptionsTicker ticker,
        int multiplier,
        AggregateInterval timespan,
        string from,
        string to,
        bool? adjusted = null,
        SortOrder? sort = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);
        ArgumentNullException.ThrowIfNull(ticker);

        var request = new GetBarsRequest
        {
            OptionsTicker = ticker.ToString(),
            Multiplier = multiplier,
            Timespan = timespan,
            From = from,
            To = to,
            Adjusted = adjusted,
            Sort = sort,
            Limit = limit
        };
        return optionsService.GetBarsAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves the daily open, high, low, close (OHLC) summary for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns comprehensive daily trading data including opening and closing prices, high and low prices, trading volume, and pre-market and after-hours prices.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="date">The date of the requested daily data in YYYY-MM-DD format (e.g., "2023-01-09").</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the daily OHLC summary.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService or ticker is null.</exception>
    /// <example>
    /// <code>
    /// var ticker = OptionsTicker.Parse("O:SPY251219C00650000");
    /// var daily = await client.Options.GetDailyOpenCloseAsync(ticker, "2023-01-09");
    /// </code>
    /// </example>
    public static Task<OptionDailyOpenClose> GetDailyOpenCloseAsync(
        this IOptionsService optionsService,
        OptionsTicker ticker,
        string date,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);
        ArgumentNullException.ThrowIfNull(ticker);

        var request = new GetDailyOpenCloseRequest
        {
            OptionsTicker = ticker.ToString(),
            Date = date
        };
        return optionsService.GetDailyOpenCloseAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves the previous trading day's OHLC data for a specific options contract using an <see cref="OptionsTicker"/> object.
    /// This is a convenience method that accepts a strongly-typed options ticker object.
    /// Returns the most recent completed trading session's open, high, low, close, volume, and volume-weighted average price data.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="ticker">The options ticker object containing the contract details.</param>
    /// <param name="adjusted">Whether to adjust for underlying stock splits. Defaults to true if not specified.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the previous trading day's OHLC bar data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService or ticker is null.</exception>
    /// <example>
    /// <code>
    /// var ticker = new OptionsTicker("SPY", new DateTime(2025, 12, 19), OptionType.Call, 650m);
    /// var previousDay = await client.Options.GetPreviousDayBarAsync(ticker);
    /// </code>
    /// </example>
    public static Task<PolygonResponse<List<OptionBar>>> GetPreviousDayBarAsync(
        this IOptionsService optionsService,
        OptionsTicker ticker,
        bool? adjusted = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);
        ArgumentNullException.ThrowIfNull(ticker);

        var request = new GetPreviousDayBarRequest
        {
            OptionsTicker = ticker.ToString(),
            Adjusted = adjusted
        };
        return optionsService.GetPreviousDayBarAsync(request, cancellationToken);
    }

    #endregion

    /// <summary>
    /// Retrieves the most recent trade for a specific options contract using its component parts.
    /// This is a convenience method that constructs the OCC format ticker string from the provided components.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "TSLA", "SPY").</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with the most recent trade data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService is null.</exception>
    /// <example>
    /// <code>
    /// // Get last trade for TSLA March 20, 2026 $700 Call
    /// var trade = await client.Options.GetLastTradeByComponentsAsync(
    ///     "TSLA",
    ///     new DateTime(2026, 3, 20),
    ///     OptionType.Call,
    ///     700m
    /// );
    /// </code>
    /// </example>
    public static Task<PolygonResponse<OptionTrade>> GetLastTradeByComponentsAsync(
        this IOptionsService optionsService,
        string underlying,
        DateTime expirationDate,
        OptionType type,
        decimal strike,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);

        string ticker = OptionsTicker.Create(underlying, expirationDate, type, strike);
        var request = new GetLastTradeRequest { OptionsTicker = ticker };
        return optionsService.GetLastTradeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves aggregate OHLC (bar/candle) data for an options contract using its component parts.
    /// This is a convenience method that constructs the OCC format ticker string from the provided components.
    /// </summary>
    /// <param name="optionsService">The options service instance.</param>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "SPY").</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract.</param>
    /// <param name="multiplier">The number of timespan units to aggregate (e.g., 1 for 1 day, 5 for 5 minutes).</param>
    /// <param name="timespan">The size of the time window for each aggregate (e.g., minute, hour, day).</param>
    /// <param name="from">Start date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="to">End date for the aggregate window in YYYY-MM-DD format.</param>
    /// <param name="adjusted">Whether to adjust for splits. Defaults to true if not specified.</param>
    /// <param name="sort">Sort order for results (asc for ascending, desc for descending by timestamp).</param>
    /// <param name="limit">Limit the number of aggregate results returned.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response with a list of aggregate OHLC bars.</returns>
    /// <exception cref="ArgumentNullException">Thrown when optionsService is null.</exception>
    /// <example>
    /// <code>
    /// // Get daily bars for SPY December 19, 2025 $650 Call
    /// var bars = await client.Options.GetBarsByComponentsAsync(
    ///     "SPY",
    ///     new DateTime(2025, 12, 19),
    ///     OptionType.Call,
    ///     650m,
    ///     multiplier: 1,
    ///     timespan: AggregateInterval.Day,
    ///     from: "2025-11-01",
    ///     to: "2025-11-30"
    /// );
    /// </code>
    /// </example>
    public static Task<PolygonResponse<List<OptionBar>>> GetBarsByComponentsAsync(
        this IOptionsService optionsService,
        string underlying,
        DateTime expirationDate,
        OptionType type,
        decimal strike,
        int multiplier,
        AggregateInterval timespan,
        string from,
        string to,
        bool? adjusted = null,
        SortOrder? sort = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(optionsService);

        string ticker = OptionsTicker.Create(underlying, expirationDate, type, strike);
        var request = new GetBarsRequest
        {
            OptionsTicker = ticker,
            Multiplier = multiplier,
            Timespan = timespan,
            From = from,
            To = to,
            Adjusted = adjusted,
            Sort = sort,
            Limit = limit
        };
        return optionsService.GetBarsAsync(request, cancellationToken);
    }
}
