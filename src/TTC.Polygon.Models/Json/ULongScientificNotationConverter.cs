// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TTC.Polygon.Models.Json;

/// <summary>
/// Custom JSON converter for ulong values that can handle scientific notation.
/// System.Text.Json requires scientific notation to be parsed as floating point first,
/// then converted to the target integer type.
/// </summary>
public class ULongScientificNotationConverter : JsonConverter<ulong?>
{
    /// <summary>
    /// Reads and converts a JSON value to a nullable ulong.
    /// Handles both regular numbers and scientific notation.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert to.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The converted ulong value or null.</returns>
    public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.Number:
                // Try to get as ulong directly first
                if (reader.TryGetUInt64(out var ulongValue))
                {
                    return ulongValue;
                }

                // If that fails, it might be in scientific notation, so get as double first
                if (reader.TryGetDouble(out var doubleValue))
                {
                    return (ulong)doubleValue;
                }
                break;

            case JsonTokenType.String:
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return null;
                }

                // Try parsing as ulong directly
                if (ulong.TryParse(stringValue, out ulongValue))
                {
                    return ulongValue;
                }

                // Try parsing as double (for scientific notation) then convert
                if (double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue))
                {
                    return (ulong)doubleValue;
                }
                break;
        }

        throw new JsonException($"Unable to convert JSON value to ulong. Token type: {reader.TokenType}");
    }

    /// <summary>
    /// Writes a nullable ulong value to JSON.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The ulong value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}