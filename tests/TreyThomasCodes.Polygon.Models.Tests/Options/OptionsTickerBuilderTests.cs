// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Options;

namespace TreyThomasCodes.Polygon.Models.Tests.Options;

/// <summary>
/// Unit tests for the OptionsTickerBuilder class.
/// </summary>
public class OptionsTickerBuilderTests
{
    #region Build Method Tests

    [Fact]
    public void Build_WithAllPropertiesSet_ReturnsCorrectTicker()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("UBER")
            .WithExpiration(new DateOnly(2022, 1, 21))
            .AsCall()
            .WithStrike(50m)
            .Build();

        Assert.Equal("O:UBER220121C00050000", ticker);
    }

    [Fact]
    public void Build_WithSeparateDateComponents_ReturnsCorrectTicker()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("SPY")
            .WithExpiration(2025, 12, 19)
            .AsPut()
            .WithStrike(650m)
            .Build();

        Assert.Equal("O:SPY251219P00650000", ticker);
    }

    [Fact]
    public void Build_WithExplicitType_ReturnsCorrectTicker()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("TSLA")
            .WithExpiration(2026, 3, 20)
            .WithType(OptionType.Call)
            .WithStrike(700m)
            .Build();

        Assert.Equal("O:TSLA260320C00700000", ticker);
    }

    [Fact]
    public void Build_WithoutUnderlying_ThrowsInvalidOperationException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<InvalidOperationException>(() =>
            builder
                .WithExpiration(2022, 1, 21)
                .AsCall()
                .WithStrike(50m)
                .Build());
    }

    [Fact]
    public void Build_WithoutExpiration_ThrowsInvalidOperationException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<InvalidOperationException>(() =>
            builder
                .WithUnderlying("UBER")
                .AsCall()
                .WithStrike(50m)
                .Build());
    }

    [Fact]
    public void Build_WithoutType_ThrowsInvalidOperationException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<InvalidOperationException>(() =>
            builder
                .WithUnderlying("UBER")
                .WithExpiration(2022, 1, 21)
                .WithStrike(50m)
                .Build());
    }

    [Fact]
    public void Build_WithoutStrike_ThrowsInvalidOperationException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<InvalidOperationException>(() =>
            builder
                .WithUnderlying("UBER")
                .WithExpiration(2022, 1, 21)
                .AsCall()
                .Build());
    }

    #endregion

    #region BuildTicker Method Tests

    [Fact]
    public void BuildTicker_WithAllPropertiesSet_ReturnsOptionsTickerInstance()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("UBER")
            .WithExpiration(new DateOnly(2022, 1, 21))
            .AsCall()
            .WithStrike(50m)
            .BuildTicker();

        Assert.NotNull(ticker);
        Assert.Equal("UBER", ticker.Underlying);
        Assert.Equal(new DateOnly(2022, 1, 21), ticker.ExpirationDate);
        Assert.Equal(OptionType.Call, ticker.Type);
        Assert.Equal(50m, ticker.Strike);
    }

    [Fact]
    public void BuildTicker_WithoutUnderlying_ThrowsInvalidOperationException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<InvalidOperationException>(() =>
            builder
                .WithExpiration(2022, 1, 21)
                .AsCall()
                .WithStrike(50m)
                .BuildTicker());
    }

    #endregion

    #region Fluent Interface Tests

    [Fact]
    public void FluentInterface_MethodsReturnBuilder_AllowsChaining()
    {
        var builder = new OptionsTickerBuilder();

        var result = builder
            .WithUnderlying("UBER")
            .WithExpiration(2022, 1, 21)
            .AsCall()
            .WithStrike(50m);

        Assert.IsType<OptionsTickerBuilder>(result);
    }

    [Fact]
    public void FluentInterface_DifferentMethodOrders_ProducesSameResult()
    {
        var ticker1 = new OptionsTickerBuilder()
            .WithUnderlying("SPY")
            .WithStrike(650m)
            .WithExpiration(2025, 12, 19)
            .AsCall()
            .Build();

        var ticker2 = new OptionsTickerBuilder()
            .AsCall()
            .WithExpiration(2025, 12, 19)
            .WithStrike(650m)
            .WithUnderlying("SPY")
            .Build();

        Assert.Equal(ticker1, ticker2);
    }

    #endregion

    #region WithUnderlying Tests

    [Fact]
    public void WithUnderlying_WithValidSymbol_SetsUnderlying()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("AAPL")
            .WithExpiration(2025, 1, 17)
            .AsCall()
            .WithStrike(150m)
            .Build();

        Assert.Contains("AAPL", ticker);
    }

    [Fact]
    public void WithUnderlying_WithNullSymbol_ThrowsArgumentNullException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.WithUnderlying(null!));
    }

    [Fact]
    public void WithUnderlying_WithEmptySymbol_ThrowsArgumentNullException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.WithUnderlying(""));
    }

    [Fact]
    public void WithUnderlying_WithWhitespaceSymbol_ThrowsArgumentNullException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.WithUnderlying("   "));
    }

    #endregion

    #region WithExpiration Tests

    [Fact]
    public void WithExpiration_WithDateTime_SetsExpiration()
    {
        var expirationDate = new DateOnly(2025, 6, 20);
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("MSFT")
            .WithExpiration(expirationDate)
            .AsCall()
            .WithStrike(400m)
            .Build();

        Assert.Contains("250620", ticker); // YYMMDD format
    }

    [Fact]
    public void WithExpiration_WithYearMonthDay_SetsExpiration()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("GOOGL")
            .WithExpiration(2025, 9, 19)
            .AsCall()
            .WithStrike(2800m)
            .Build();

        Assert.Contains("250919", ticker); // YYMMDD format
    }

    [Fact]
    public void WithExpiration_WithInvalidDate_ThrowsArgumentOutOfRangeException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.WithExpiration(2025, 13, 1)); // Invalid month
    }

    #endregion

    #region AsCall and AsPut Tests

    [Fact]
    public void AsCall_SetsTypeToCall()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("UBER")
            .WithExpiration(2022, 1, 21)
            .AsCall()
            .WithStrike(50m)
            .Build();

        Assert.Contains("C", ticker);
        Assert.DoesNotContain("P", ticker.Substring(ticker.IndexOf('C'))); // Ensure P is not after C
    }

    [Fact]
    public void AsPut_SetsTypeToPut()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("UBER")
            .WithExpiration(2022, 1, 21)
            .AsPut()
            .WithStrike(50m)
            .Build();

        Assert.Contains("P", ticker);
    }

    [Fact]
    public void WithType_WithCall_SetsTypeToCall()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("UBER")
            .WithExpiration(2022, 1, 21)
            .WithType(OptionType.Call)
            .WithStrike(50m)
            .Build();

        Assert.Contains("C", ticker);
    }

    [Fact]
    public void WithType_WithPut_SetsTypeToPut()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("UBER")
            .WithExpiration(2022, 1, 21)
            .WithType(OptionType.Put)
            .WithStrike(50m)
            .Build();

        Assert.Contains("P", ticker);
    }

    #endregion

    #region WithStrike Tests

    [Fact]
    public void WithStrike_WithWholeNumber_SetsStrike()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("UBER")
            .WithExpiration(2022, 1, 21)
            .AsCall()
            .WithStrike(50m)
            .Build();

        Assert.Contains("00050000", ticker); // $50 = 00050000
    }

    [Fact]
    public void WithStrike_WithDecimal_SetsStrike()
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying("AAPL")
            .WithExpiration(2024, 12, 20)
            .AsCall()
            .WithStrike(150.50m)
            .Build();

        Assert.Contains("00150500", ticker); // $150.50 = 00150500
    }

    [Fact]
    public void WithStrike_WithNegativeValue_ThrowsArgumentException()
    {
        var builder = new OptionsTickerBuilder();

        Assert.Throws<ArgumentException>(() => builder.WithStrike(-10m));
    }

    #endregion

    #region Reset Method Tests

    [Fact]
    public void Reset_ClearsAllProperties()
    {
        var builder = new OptionsTickerBuilder();
        builder
            .WithUnderlying("UBER")
            .WithExpiration(2022, 1, 21)
            .AsCall()
            .WithStrike(50m)
            .Reset();

        // Should throw because properties are cleared
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Reset_AllowsReuse_ForNewTicker()
    {
        var builder = new OptionsTickerBuilder();

        var ticker1 = builder
            .WithUnderlying("UBER")
            .WithExpiration(2022, 1, 21)
            .AsCall()
            .WithStrike(50m)
            .Build();

        var ticker2 = builder
            .Reset()
            .WithUnderlying("SPY")
            .WithExpiration(2025, 12, 19)
            .AsPut()
            .WithStrike(650m)
            .Build();

        Assert.Equal("O:UBER220121C00050000", ticker1);
        Assert.Equal("O:SPY251219P00650000", ticker2);
        Assert.NotEqual(ticker1, ticker2);
    }

    #endregion

    #region Complex Scenarios

    [Theory]
    [InlineData("UBER", 2022, 1, 21, OptionType.Call, 50, "O:UBER220121C00050000")]
    [InlineData("F", 2021, 11, 19, OptionType.Put, 14, "O:F211119P00014000")]
    [InlineData("SPY", 2025, 12, 19, OptionType.Call, 650, "O:SPY251219C00650000")]
    [InlineData("TSLA", 2026, 3, 20, OptionType.Put, 700, "O:TSLA260320P00700000")]
    public void Build_WithVariousInputs_ProducesExpectedTickers(
        string underlying, int year, int month, int day, OptionType type, decimal strike, string expected)
    {
        var builder = new OptionsTickerBuilder();
        var ticker = builder
            .WithUnderlying(underlying)
            .WithExpiration(year, month, day)
            .WithType(type)
            .WithStrike(strike)
            .Build();

        Assert.Equal(expected, ticker);
    }

    [Fact]
    public void Build_ComparedWithOptionsTicker_ProducesSameResult()
    {
        var underlying = "AAPL";
        var expiration = new DateOnly(2024, 12, 20);
        var type = OptionType.Call;
        var strike = 150m;

        var builderResult = new OptionsTickerBuilder()
            .WithUnderlying(underlying)
            .WithExpiration(expiration)
            .WithType(type)
            .WithStrike(strike)
            .Build();

        var directResult = OptionsTicker.Create(underlying, expiration, type, strike);

        Assert.Equal(directResult, builderResult);
    }

    #endregion
}
