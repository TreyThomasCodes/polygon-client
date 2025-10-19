// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

namespace TreyThomasCodes.Polygon.Models.Options;

/// <summary>
/// Provides a fluent API for building OCC format options ticker strings.
/// This builder pattern offers a more discoverable and readable way to construct options tickers.
/// </summary>
/// <example>
/// <code>
/// var ticker = new OptionsTickerBuilder()
///     .WithUnderlying("UBER")
///     .WithExpiration(2022, 1, 21)
///     .AsCall()
///     .WithStrike(50m)
///     .Build();
/// // Returns: "O:UBER220121C00050000"
/// </code>
/// </example>
public class OptionsTickerBuilder
{
    private string? _underlying;
    private DateOnly? _expirationDate;
    private OptionType? _type;
    private decimal? _strike;

    /// <summary>
    /// Sets the underlying asset ticker symbol.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "UBER", "SPY", "AAPL").</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when underlying is null or whitespace.</exception>
    public OptionsTickerBuilder WithUnderlying(string underlying)
    {
        if (string.IsNullOrWhiteSpace(underlying))
            throw new ArgumentNullException(nameof(underlying), "Underlying ticker symbol cannot be null or empty.");

        _underlying = underlying;
        return this;
    }

    /// <summary>
    /// Sets the expiration date of the options contract.
    /// </summary>
    /// <param name="expirationDate">The expiration date.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public OptionsTickerBuilder WithExpiration(DateOnly expirationDate)
    {
        _expirationDate = expirationDate;
        return this;
    }

    /// <summary>
    /// Sets the expiration date of the options contract using year, month, and day components.
    /// </summary>
    /// <param name="year">The year (e.g., 2022).</param>
    /// <param name="month">The month (1-12).</param>
    /// <param name="day">The day of the month.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the date components are invalid.</exception>
    public OptionsTickerBuilder WithExpiration(int year, int month, int day)
    {
        _expirationDate = new DateOnly(year, month, day);
        return this;
    }

    /// <summary>
    /// Sets the options contract type to Call.
    /// A call option gives the holder the right to buy the underlying asset at the strike price.
    /// </summary>
    /// <returns>The current builder instance for method chaining.</returns>
    public OptionsTickerBuilder AsCall()
    {
        _type = OptionType.Call;
        return this;
    }

    /// <summary>
    /// Sets the options contract type to Put.
    /// A put option gives the holder the right to sell the underlying asset at the strike price.
    /// </summary>
    /// <returns>The current builder instance for method chaining.</returns>
    public OptionsTickerBuilder AsPut()
    {
        _type = OptionType.Put;
        return this;
    }

    /// <summary>
    /// Sets the options contract type explicitly.
    /// </summary>
    /// <param name="type">The option type (Call or Put).</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public OptionsTickerBuilder WithType(OptionType type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Sets the strike price of the options contract.
    /// </summary>
    /// <param name="strike">The strike price (e.g., 50 for $50, 14.5 for $14.50).</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when strike price is negative.</exception>
    public OptionsTickerBuilder WithStrike(decimal strike)
    {
        if (strike < 0)
            throw new ArgumentException("Strike price cannot be negative.", nameof(strike));

        _strike = strike;
        return this;
    }

    /// <summary>
    /// Builds and returns the OCC format options ticker string.
    /// </summary>
    /// <returns>A formatted OCC options ticker string (e.g., "O:UBER220121C00050000").</returns>
    /// <exception cref="InvalidOperationException">Thrown when required properties (Underlying, ExpirationDate, Type, or Strike) have not been set.</exception>
    /// <example>
    /// <code>
    /// // Build a Call option ticker
    /// var callTicker = new OptionsTickerBuilder()
    ///     .WithUnderlying("SPY")
    ///     .WithExpiration(2025, 12, 19)
    ///     .AsCall()
    ///     .WithStrike(650m)
    ///     .Build();
    /// // Returns: "O:SPY251219C00650000"
    ///
    /// // Build a Put option ticker
    /// var putTicker = new OptionsTickerBuilder()
    ///     .WithUnderlying("TSLA")
    ///     .WithExpiration(new DateOnly(2026, 3, 20))
    ///     .AsPut()
    ///     .WithStrike(700m)
    ///     .Build();
    /// // Returns: "O:TSLA260320P00700000"
    /// </code>
    /// </example>
    public string Build()
    {
        if (_underlying == null)
            throw new InvalidOperationException("Underlying ticker symbol must be set before building. Call WithUnderlying() first.");

        if (_expirationDate == null)
            throw new InvalidOperationException("Expiration date must be set before building. Call WithExpiration() first.");

        if (_type == null)
            throw new InvalidOperationException("Option type must be set before building. Call AsCall(), AsPut(), or WithType() first.");

        if (_strike == null)
            throw new InvalidOperationException("Strike price must be set before building. Call WithStrike() first.");

        return OptionsTicker.Create(_underlying, _expirationDate.Value, _type.Value, _strike.Value);
    }

    /// <summary>
    /// Builds and returns an <see cref="OptionsTicker"/> instance with the configured properties.
    /// </summary>
    /// <returns>An <see cref="OptionsTicker"/> instance containing the parsed components.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required properties (Underlying, ExpirationDate, Type, or Strike) have not been set.</exception>
    public OptionsTicker BuildTicker()
    {
        if (_underlying == null)
            throw new InvalidOperationException("Underlying ticker symbol must be set before building. Call WithUnderlying() first.");

        if (_expirationDate == null)
            throw new InvalidOperationException("Expiration date must be set before building. Call WithExpiration() first.");

        if (_type == null)
            throw new InvalidOperationException("Option type must be set before building. Call AsCall(), AsPut(), or WithType() first.");

        if (_strike == null)
            throw new InvalidOperationException("Strike price must be set before building. Call WithStrike() first.");

        return new OptionsTicker(_underlying, _expirationDate.Value, _type.Value, _strike.Value);
    }

    /// <summary>
    /// Resets all properties of the builder to their default values.
    /// Allows the builder to be reused for constructing multiple tickers.
    /// </summary>
    /// <returns>The current builder instance for method chaining.</returns>
    public OptionsTickerBuilder Reset()
    {
        _underlying = null;
        _expirationDate = null;
        _type = null;
        _strike = null;
        return this;
    }
}
