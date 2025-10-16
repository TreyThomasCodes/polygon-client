// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Options;

namespace TreyThomasCodes.Polygon.Models.Tests.Options;

/// <summary>
/// Unit tests for the OptionsTicker class.
/// </summary>
public class OptionsTickerTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        var underlying = "UBER";
        var expirationDate = new DateTime(2022, 1, 21);
        var type = OptionType.Call;
        var strike = 50m;

        var ticker = new OptionsTicker(underlying, expirationDate, type, strike);

        Assert.Equal("UBER", ticker.Underlying);
        Assert.Equal(new DateTime(2022, 1, 21), ticker.ExpirationDate);
        Assert.Equal(OptionType.Call, ticker.Type);
        Assert.Equal(50m, ticker.Strike);
    }

    [Fact]
    public void Constructor_WithLowercaseUnderlying_ConvertsToUppercase()
    {
        var ticker = new OptionsTicker("uber", new DateTime(2022, 1, 21), OptionType.Call, 50m);

        Assert.Equal("UBER", ticker.Underlying);
    }

    [Fact]
    public void Constructor_WithNullUnderlying_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OptionsTicker(null!, new DateTime(2022, 1, 21), OptionType.Call, 50m));
    }

    [Fact]
    public void Constructor_WithEmptyUnderlying_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OptionsTicker("", new DateTime(2022, 1, 21), OptionType.Call, 50m));
    }

    [Fact]
    public void Constructor_WithWhitespaceUnderlying_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OptionsTicker("   ", new DateTime(2022, 1, 21), OptionType.Call, 50m));
    }

    [Fact]
    public void Constructor_WithInvalidCharactersInUnderlying_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new OptionsTicker("UBER123", new DateTime(2022, 1, 21), OptionType.Call, 50m));
    }

    [Fact]
    public void Constructor_WithNegativeStrike_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, -10m));
    }

    #endregion

    #region Create Method Tests

    [Fact]
    public void Create_WithValidParameters_ReturnsCorrectlyFormattedTicker()
    {
        var ticker = OptionsTicker.Create("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);

        Assert.Equal("O:UBER220121C00050000", ticker);
    }

    [Fact]
    public void Create_WithPutOption_ReturnsCorrectlyFormattedTicker()
    {
        var ticker = OptionsTicker.Create("F", new DateTime(2021, 11, 19), OptionType.Put, 14m);

        Assert.Equal("O:F211119P00014000", ticker);
    }

    [Fact]
    public void Create_WithSingleCharacterUnderlying_ReturnsCorrectlyFormattedTicker()
    {
        var ticker = OptionsTicker.Create("F", new DateTime(2021, 11, 19), OptionType.Call, 15.50m);

        Assert.Equal("O:F211119C00015500", ticker);
    }

    [Fact]
    public void Create_WithHighStrikePrice_ReturnsCorrectlyFormattedTicker()
    {
        var ticker = OptionsTicker.Create("SPY", new DateTime(2025, 12, 19), OptionType.Call, 650m);

        Assert.Equal("O:SPY251219C00650000", ticker);
    }

    [Fact]
    public void Create_WithDecimalStrikePrice_ReturnsCorrectlyFormattedTicker()
    {
        var ticker = OptionsTicker.Create("TSLA", new DateTime(2026, 3, 20), OptionType.Put, 700.50m);

        Assert.Equal("O:TSLA260320P00700500", ticker);
    }

    [Fact]
    public void Create_WithLowercaseUnderlying_ConvertsToUppercase()
    {
        var ticker = OptionsTicker.Create("uber", new DateTime(2022, 1, 21), OptionType.Call, 50m);

        Assert.Equal("O:UBER220121C00050000", ticker);
    }

    #endregion

    #region Parse Method Tests

    [Fact]
    public void Parse_WithValidCallTicker_ParsesCorrectly()
    {
        var ticker = OptionsTicker.Parse("O:UBER220121C00050000");

        Assert.Equal("UBER", ticker.Underlying);
        Assert.Equal(new DateTime(2022, 1, 21), ticker.ExpirationDate);
        Assert.Equal(OptionType.Call, ticker.Type);
        Assert.Equal(50m, ticker.Strike);
    }

    [Fact]
    public void Parse_WithValidPutTicker_ParsesCorrectly()
    {
        var ticker = OptionsTicker.Parse("O:F211119P00014000");

        Assert.Equal("F", ticker.Underlying);
        Assert.Equal(new DateTime(2021, 11, 19), ticker.ExpirationDate);
        Assert.Equal(OptionType.Put, ticker.Type);
        Assert.Equal(14m, ticker.Strike);
    }

    [Fact]
    public void Parse_WithDecimalStrikePrice_ParsesCorrectly()
    {
        var ticker = OptionsTicker.Parse("O:SPY251219C00650500");

        Assert.Equal("SPY", ticker.Underlying);
        Assert.Equal(new DateTime(2025, 12, 19), ticker.ExpirationDate);
        Assert.Equal(OptionType.Call, ticker.Type);
        Assert.Equal(650.50m, ticker.Strike);
    }

    [Fact]
    public void Parse_WithInvalidFormat_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => OptionsTicker.Parse("INVALID"));
    }

    [Fact]
    public void Parse_WithMissingPrefix_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => OptionsTicker.Parse("UBER220121C00050000"));
    }

    [Fact]
    public void Parse_WithNullTicker_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => OptionsTicker.Parse(null!));
    }

    [Fact]
    public void Parse_WithEmptyTicker_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => OptionsTicker.Parse(""));
    }

    #endregion

    #region TryParse Method Tests

    [Fact]
    public void TryParse_WithValidCallTicker_ReturnsTrue()
    {
        var success = OptionsTicker.TryParse("O:UBER220121C00050000", out var ticker);

        Assert.True(success);
        Assert.NotNull(ticker);
        Assert.Equal("UBER", ticker.Underlying);
        Assert.Equal(new DateTime(2022, 1, 21), ticker.ExpirationDate);
        Assert.Equal(OptionType.Call, ticker.Type);
        Assert.Equal(50m, ticker.Strike);
    }

    [Fact]
    public void TryParse_WithValidPutTicker_ReturnsTrue()
    {
        var success = OptionsTicker.TryParse("O:F211119P00014000", out var ticker);

        Assert.True(success);
        Assert.NotNull(ticker);
        Assert.Equal("F", ticker.Underlying);
        Assert.Equal(OptionType.Put, ticker.Type);
    }

    [Fact]
    public void TryParse_WithInvalidFormat_ReturnsFalse()
    {
        var success = OptionsTicker.TryParse("INVALID", out var ticker);

        Assert.False(success);
        Assert.Null(ticker);
    }

    [Fact]
    public void TryParse_WithNullTicker_ReturnsFalse()
    {
        var success = OptionsTicker.TryParse(null, out var ticker);

        Assert.False(success);
        Assert.Null(ticker);
    }

    [Fact]
    public void TryParse_WithEmptyTicker_ReturnsFalse()
    {
        var success = OptionsTicker.TryParse("", out var ticker);

        Assert.False(success);
        Assert.Null(ticker);
    }

    [Fact]
    public void TryParse_WithMissingPrefix_ReturnsFalse()
    {
        var success = OptionsTicker.TryParse("UBER220121C00050000", out var ticker);

        Assert.False(success);
        Assert.Null(ticker);
    }

    #endregion

    #region ToString Method Tests

    [Fact]
    public void ToString_WithCallOption_ReturnsCorrectFormat()
    {
        var ticker = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);

        Assert.Equal("O:UBER220121C00050000", ticker.ToString());
    }

    [Fact]
    public void ToString_WithPutOption_ReturnsCorrectFormat()
    {
        var ticker = new OptionsTicker("F", new DateTime(2021, 11, 19), OptionType.Put, 14m);

        Assert.Equal("O:F211119P00014000", ticker.ToString());
    }

    [Fact]
    public void ToString_WithDecimalStrike_ReturnsCorrectFormat()
    {
        var ticker = new OptionsTicker("SPY", new DateTime(2025, 12, 19), OptionType.Call, 650.50m);

        Assert.Equal("O:SPY251219C00650500", ticker.ToString());
    }

    #endregion

    #region Round-trip Tests

    [Theory]
    [InlineData("O:UBER220121C00050000")]
    [InlineData("O:F211119P00014000")]
    [InlineData("O:SPY251219C00650000")]
    [InlineData("O:TSLA260320P00700000")]
    [InlineData("O:AAPL241220C00150500")]
    public void RoundTrip_ParseAndToString_ReturnsOriginalTicker(string originalTicker)
    {
        var parsed = OptionsTicker.Parse(originalTicker);
        var reconstructed = parsed.ToString();

        Assert.Equal(originalTicker, reconstructed);
    }

    [Theory]
    [InlineData("UBER", 2022, 1, 21, OptionType.Call, 50)]
    [InlineData("F", 2021, 11, 19, OptionType.Put, 14)]
    [InlineData("SPY", 2025, 12, 19, OptionType.Call, 650)]
    [InlineData("TSLA", 2026, 3, 20, OptionType.Put, 700)]
    public void RoundTrip_CreateAndParse_ReturnsSameComponents(
        string underlying, int year, int month, int day, OptionType type, decimal strike)
    {
        var expirationDate = new DateTime(year, month, day);
        var ticker = OptionsTicker.Create(underlying, expirationDate, type, strike);
        var parsed = OptionsTicker.Parse(ticker);

        Assert.Equal(underlying, parsed.Underlying);
        Assert.Equal(expirationDate, parsed.ExpirationDate);
        Assert.Equal(type, parsed.Type);
        Assert.Equal(strike, parsed.Strike);
    }

    #endregion

    #region Equals and GetHashCode Tests

    [Fact]
    public void Equals_WithIdenticalTickers_ReturnsTrue()
    {
        var ticker1 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);
        var ticker2 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);

        Assert.True(ticker1.Equals(ticker2));
    }

    [Fact]
    public void Equals_WithDifferentUnderlying_ReturnsFalse()
    {
        var ticker1 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);
        var ticker2 = new OptionsTicker("LYFT", new DateTime(2022, 1, 21), OptionType.Call, 50m);

        Assert.False(ticker1.Equals(ticker2));
    }

    [Fact]
    public void Equals_WithDifferentExpiration_ReturnsFalse()
    {
        var ticker1 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);
        var ticker2 = new OptionsTicker("UBER", new DateTime(2022, 1, 28), OptionType.Call, 50m);

        Assert.False(ticker1.Equals(ticker2));
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        var ticker1 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);
        var ticker2 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Put, 50m);

        Assert.False(ticker1.Equals(ticker2));
    }

    [Fact]
    public void Equals_WithDifferentStrike_ReturnsFalse()
    {
        var ticker1 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);
        var ticker2 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 55m);

        Assert.False(ticker1.Equals(ticker2));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var ticker1 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);

        Assert.False(ticker1.Equals(null));
    }

    [Fact]
    public void GetHashCode_WithIdenticalTickers_ReturnsSameHashCode()
    {
        var ticker1 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);
        var ticker2 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);

        Assert.Equal(ticker1.GetHashCode(), ticker2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentTickers_ReturnsDifferentHashCode()
    {
        var ticker1 = new OptionsTicker("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);
        var ticker2 = new OptionsTicker("LYFT", new DateTime(2022, 1, 21), OptionType.Call, 50m);

        Assert.NotEqual(ticker1.GetHashCode(), ticker2.GetHashCode());
    }

    #endregion
}
