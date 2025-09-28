using System.Runtime.InteropServices;
using System.Text.Json;
using TTC.Polygon.Models.Json;

namespace TTC.Polygon.Models.Tests.Json;

/// <summary>
/// Unit tests for ULongScientificNotationConverter.
/// </summary>
public class ULongScientificNotationConverterTests
{
    private readonly ULongScientificNotationConverter _converter;
    private readonly JsonSerializerOptions _options;

    public ULongScientificNotationConverterTests()
    {
        _converter = new ULongScientificNotationConverter();
        _options = new JsonSerializerOptions();
        _options.Converters.Add(_converter);
    }

    [Fact]
    public void Read_WithNullToken_ReturnsNull()
    {
        var json = "null";

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Null(result);
    }

    [Fact]
    public void Read_WithRegularNumber_ReturnsCorrectValue()
    {
        var json = "12345";

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(12345UL, result);
    }

    [Fact]
    public void Read_WithLargeNumber_ReturnsCorrectValue()
    {
        var json = "18446744073709551615"; // ulong.MaxValue

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(ulong.MaxValue, result);
    }

    [Fact]
    public void Read_WithZero_ReturnsZero()
    {
        var json = "0";

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(0UL, result);
    }

    [Fact]
    public void Read_WithScientificNotationNumber_ReturnsCorrectValue()
    {
        var json = "1.23e6"; // 1,230,000

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(1230000UL, result);
    }

    [Fact]
    public void Read_WithLargeScientificNotationNumber_ReturnsCorrectValue()
    {
        var json = "1.5e15"; // 1,500,000,000,000,000

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(1500000000000000UL, result);
    }

    [Fact]
    public void Read_WithStringNumber_ReturnsCorrectValue()
    {
        var json = "\"12345\"";

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(12345UL, result);
    }

    [Fact]
    public void Read_WithStringScientificNotation_ReturnsCorrectValue()
    {
        var json = "\"1.23e6\"";

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(1230000UL, result);
    }

    [Fact]
    public void Read_WithEmptyString_ReturnsNull()
    {
        var json = "\"\"";

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Null(result);
    }

    [Fact]
    public void Read_WithWhitespaceString_ThrowsJsonException()
    {
        var json = "\"   \"";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong?>(json, _options));
    }

    [Fact]
    public void Read_WithInvalidString_ThrowsJsonException()
    {
        var json = "\"invalid\"";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong?>(json, _options));
    }

#if NET8_0
    [Fact]
    public void Read_WithNegativeNumber_Net8_ReturnsWrappedValue()
    {
        var json = "-123";

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        // In .NET 8, negative numbers wrap around to large ulong values
        Assert.Equal(18446744073709551493UL, result);
    }
#endif

#if NET9_0_OR_GREATER
    [Fact]
    public void Read_WithNegativeNumber_Net9_ReturnsZero()
    {
        var json = "-123";

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        // In .NET 9+, negative numbers are converted to 0
        Assert.Equal(0UL, result);
    }
#endif

    [Fact]
    public void Read_WithFloatingPointNumber_TruncatesCorrectly()
    {
        var json = "123.7";

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(123UL, result);
    }

    [Fact]
    public void Read_WithBooleanToken_ThrowsJsonException()
    {
        var json = "true";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong?>(json, _options));
    }

    [Fact]
    public void Read_WithObjectToken_ThrowsJsonException()
    {
        var json = "{}";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong?>(json, _options));
    }

    [Fact]
    public void Read_WithArrayToken_ThrowsJsonException()
    {
        var json = "[]";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong?>(json, _options));
    }

    [Fact]
    public void Write_WithValue_WritesCorrectNumber()
    {
        ulong? value = 12345UL;

        var json = JsonSerializer.Serialize(value, _options);

        Assert.Equal("12345", json);
    }

    [Fact]
    public void Write_WithLargeValue_WritesCorrectNumber()
    {
        ulong? value = ulong.MaxValue;

        var json = JsonSerializer.Serialize(value, _options);

        Assert.Equal("18446744073709551615", json);
    }

    [Fact]
    public void Write_WithZero_WritesZero()
    {
        ulong? value = 0UL;

        var json = JsonSerializer.Serialize(value, _options);

        Assert.Equal("0", json);
    }

    [Fact]
    public void Write_WithNull_WritesNull()
    {
        ulong? value = null;

        var json = JsonSerializer.Serialize(value, _options);

        Assert.Equal("null", json);
    }

    [Fact]
    public void RoundTrip_WithRegularNumber_PreservesValue()
    {
        ulong? original = 12345UL;

        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(original, result);
    }

    [Fact]
    public void RoundTrip_WithLargeNumber_PreservesValue()
    {
        ulong? original = ulong.MaxValue;

        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(original, result);
    }

    [Fact]
    public void RoundTrip_WithNull_PreservesNull()
    {
        ulong? original = null;

        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(original, result);
    }

    [Theory]
    [InlineData("1e3", 1000UL)]
    [InlineData("1.5e4", 15000UL)]
    [InlineData("2.5e10", 25000000000UL)]
    [InlineData("1e0", 1UL)]
    public void Read_WithVariousScientificNotations_ReturnsCorrectValues(string input, ulong expected)
    {
        var json = input;

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("\"1e3\"", 1000UL)]
    [InlineData("\"1.5e4\"", 15000UL)]
    [InlineData("\"2.5e10\"", 25000000000UL)]
    [InlineData("\"1e0\"", 1UL)]
    public void Read_WithVariousStringScientificNotations_ReturnsCorrectValues(string input, ulong expected)
    {
        var json = input;

        var result = JsonSerializer.Deserialize<ulong?>(json, _options);

        Assert.Equal(expected, result);
    }
}